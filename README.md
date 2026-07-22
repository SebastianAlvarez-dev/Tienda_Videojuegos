# GameStore API

Backend de una tienda de videojuegos desarrollado para demostrar DDD, Arquitectura Limpia, CQRS, Entity Framework Core, PostgreSQL y pruebas de extremo a extremo con .NET Aspire.

## Requisitos

- .NET SDK 10.
- Aspire CLI 13.4 o superior.
- Docker Desktop encendido.
- Postman o Visual Studio Code con soporte para archivos `.http`.

## Inicio rápido

```powershell
dotnet tool restore
aspire start
```

Aspire levanta PostgreSQL, aplica automáticamente las migraciones de EF Core, inicia la API y abre el dashboard. La API queda disponible en `http://localhost:3000`.

No se necesita `.env` ni una cuenta de Supabase para ejecutar el examen: AppHost administra la conexión, el contenedor y sus credenciales.

## Endpoints

| Método | Ruta | Operación CQRS | Resultado |
| --- | --- | --- | --- |
| `GET` | `/` | Estado del servicio | Confirma que la API está funcionando. |
| `POST` | `/juegos` | `CrearJuegoCommand` | Registra un videojuego. |
| `GET` | `/juegos` | `ListarJuegosQuery` | Consulta el catálogo. |
| `PATCH` | `/juegos/{juegoId}` | `ActualizarJuegoCommand` | Actualiza campos recibidos. |
| `POST` | `/juegos/{juegoId}/compras` | `RegistrarCompraCommand` | Registra una venta y descuenta stock. |

## Pruebas

```powershell
dotnet test tests/unit/GameStore.UnitTests/GameStore.UnitTests.csproj
dotnet test tests/integration/GameStore.FunctionalTests/GameStore.FunctionalTests.csproj
dotnet test GameStore.slnx
```

Las pruebas funcionales usan `Aspire.Hosting.Testing`: crean el AppHost programáticamente y prueban la API real sobre PostgreSQL.

## Estructura

```text
src/
  GameStore.Domain/           Entidades, Value Objects y eventos de dominio
  GameStore.Application/      Commands, Queries, Handlers y contratos
  GameStore.Infrastructure/   EF Core, repositorio, eventos y migraciones
  GameStore.Api/              Controladores y manejo HTTP
  GameStore.AppHost/          Orquestación de API y PostgreSQL
  GameStore.ServiceDefaults/  Observabilidad, health checks y resiliencia
tests/
  unit/GameStore.UnitTests/
  integration/GameStore.FunctionalTests/
postman/
```

Consulta [MANUAL_DE_USO.md](MANUAL_DE_USO.md), [CONTEXTO.md](CONTEXTO.md) y [DOCUMENTACION.md](DOCUMENTACION.md) para la explicación completa.
