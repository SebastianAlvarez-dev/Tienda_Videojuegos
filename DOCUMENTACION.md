# Documentación técnica

## Cumplimiento de la rúbrica

| Requisito | Implementación |
| --- | --- |
| Entidades DDD | `Juego` y `Venta` con identidad y comportamiento. |
| Value Objects | `TituloJuego` y `Dinero`, inmutables y validados. |
| Eventos de dominio | `JuegoCreado`, `JuegoActualizado` y `CompraRealizada`. |
| Encapsulamiento | Setters privados, colección `IReadOnlyCollection` y métodos `Actualizar`/`Comprar`. |
| Arquitectura Limpia | Seis proyectos con dependencias dirigidas hacia el dominio. |
| CQRS | Commands/Queries y handlers separados por feature. |
| Persistencia | EF Core + Npgsql + migración `Inicial`. |
| .NET Aspire | AppHost orquesta PostgreSQL, base `gamestore` y API. |
| Testing Aspire | `DistributedApplicationTestingBuilder` levanta el sistema real. |
| Consumo API | Colección Postman y archivo `GameStore.Api.http`. |

## Dependencias entre capas

```text
GameStore.Domain
       ↑
GameStore.Application
       ↑
GameStore.Infrastructure
       ↑
GameStore.Api ← GameStore.ServiceDefaults
       ↑
GameStore.AppHost
```

`GameStore.Domain` no referencia paquetes externos ni otros proyectos. `Application` depende únicamente de `Domain`. `Infrastructure` implementa los puertos de aplicación. `Api` funciona como raíz de composición e inyecta las implementaciones.

## Modelo DDD

### Juego

- Identidad: `Guid Id`.
- Estado protegido: título, género, precio, stock y fecha de creación.
- Comportamientos: `Crear`, `Actualizar` y `Comprar`.
- Colección: `Ventas` expuesta como `IReadOnlyCollection<Venta>`.

### Venta

- Identidad: `Guid Id`.
- Pertenece a un juego mediante `JuegoId`.
- Conserva el Value Object `Dinero` usado al comprar.
- Su creación es interna al agregado `Juego`.

### Value Objects

`TituloJuego` normaliza espacios y valida longitud. `Dinero` valida que el valor sea positivo y redondea a dos decimales. Al ser records, su igualdad depende de su valor y no de una identidad.

## CQRS por feature

| Feature | Tipo | Handler |
| --- | --- | --- |
| Crear juego | Command | `CrearJuegoHandler` |
| Listar juegos | Query | `ListarJuegosHandler` |
| Actualizar juego | Command | `ActualizarJuegoHandler` |
| Registrar compra | Command | `RegistrarCompraHandler` |

Los controladores reciben handlers concretos desde DI. Las operaciones de escritura usan `IComando<TResult>` y las lecturas `IConsulta<TResult>`.

## Persistencia

`GameStoreDbContext` configura las tablas `juegos` y `ventas` con nombres de columnas en español. Los Value Objects se convierten automáticamente a tipos PostgreSQL. La migración inicial crea claves primarias, título único, relación e índices.

La API obtiene `ConnectionStrings:gamestore`, inyectada por `.WithReference(database)` desde AppHost. Después ejecuta `Database.MigrateAsync()` para mantener la base alineada con el código.

Para crear una migración nueva:

```powershell
dotnet tool restore
$env:ConnectionStrings__gamestore="Host=localhost;Port=5432;Database=gamestore;Username=postgres;Password=postgres"
dotnet tool run dotnet-ef migrations add NombreMigracion --project src/GameStore.Infrastructure --startup-project src/GameStore.Api --output-dir Persistencia/Migraciones
```

La cadena anterior solo permite construir el contexto en tiempo de diseño; AppHost administra la conexión real durante la ejecución.

## Orquestación Aspire

`GameStore.AppHost` declara:

1. Recurso PostgreSQL `postgres`.
2. Base lógica `gamestore`.
3. Proyecto `api` con referencia a la base.
4. Dependencia `WaitFor(database)`.
5. Health check `/health` y endpoints externos.

`GameStore.ServiceDefaults` añade OpenTelemetry, health checks, service discovery y resiliencia HTTP. El dashboard de Aspire permite observar estado, logs, métricas y trazas.

## Estrategia de pruebas

### Unitarias

`GameStore.UnitTests` prueba sin base de datos:

- Emisión de eventos.
- Rechazo de compras sin stock.
- Actualización parcial.
- Descuento de stock y creación de ventas.

### Funcionales/E2E

`GameStore.FunctionalTests` usa `Aspire.Hosting.Testing`. La prueba construye `GameStore.AppHost`, espera que `api` esté saludable y ejecuta el flujo `POST → PATCH → compra → GET` mediante HTTP real y PostgreSQL real.

## Respuestas HTTP

| Situación | Código |
| --- | --- |
| Consulta o actualización correcta | `200` |
| Juego o venta creados | `201` |
| Datos inválidos | `400` |
| Juego inexistente | `404` |
| Título repetido o stock insuficiente | `409` |
