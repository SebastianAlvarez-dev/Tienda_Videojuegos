# Manual de uso de GameStore API

Este manual está pensado para una persona principiante. Explica cómo abrir, ejecutar, probar y exponer el proyecto.

## 1. Herramientas necesarias

Instala:

1. **.NET SDK 10**.
2. **Aspire CLI 13.4 o superior**.
3. **Docker Desktop**.
4. **Visual Studio Code** o Visual Studio.
5. **Postman**.

Comprueba las instalaciones desde PowerShell:

```powershell
dotnet --version
aspire --version
docker --version
```

## 2. Abrir el proyecto

1. Abre Visual Studio Code.
2. Selecciona **File > Open Folder**.
3. Elige la carpeta `Examen Servidores`.
4. Abre **Terminal > New Terminal**.
5. Confirma que la terminal esté ubicada en la carpeta donde existe `GameStore.slnx`.

## 3. Preparar el entorno

Abre Docker Desktop y espera a que indique que el motor está ejecutándose. Aspire usa Docker para crear PostgreSQL automáticamente.

Restaura dependencias y herramientas:

```powershell
dotnet restore GameStore.slnx
dotnet tool restore
```

No necesitas crear `.env`, copiar contraseñas ni configurar Supabase. AppHost crea las credenciales y entrega la conexión a la API sin exponerla.

## 4. Ejecutar con .NET Aspire

Desde la raíz ejecuta:

```powershell
aspire start
```

No ejecutes AppHost con `dotnet run`; el comando correcto para este proyecto es `aspire start`.

Aspire realizará este flujo:

1. Compila la solución.
2. Crea el contenedor PostgreSQL.
3. Crea la base `gamestore`.
4. Inyecta la cadena de conexión en la API.
5. Aplica la migración de Entity Framework Core.
6. Inicia la API.
7. Abre el dashboard de Aspire.

En el dashboard deben aparecer:

- `postgres`: servidor PostgreSQL.
- `gamestore`: base de datos lógica.
- `api`: backend ASP.NET Core.

Espera a que `api` tenga estado **Healthy**. La dirección para Postman es:

```text
http://localhost:3000
```

Al abrir esa dirección en el navegador debes recibir:

```json
{"mensaje":"GameStore API funcionando correctamente"}
```

Para detener todo, vuelve a la terminal y presiona `Ctrl + C`. También puedes ejecutar:

```powershell
aspire stop
```

## 5. Probar con Postman

### Importar la colección

1. Abre Postman.
2. Pulsa **Import**.
3. Selecciona `postman/GameStore.postman_collection.json`.
4. Confirma la importación.
5. Abre la colección **GameStore API - .NET Aspire**.

La colección ya contiene:

- `baseUrl = http://localhost:3000`.
- `juegoId`, que se llena automáticamente después de registrar un juego.

### Ejecutar el flujo

Ejecuta las solicitudes en el orden numerado.

#### 1. Registrar videojuego

Envía `POST /juegos` con:

```json
{
  "titulo": "Celeste 1",
  "genero": "Plataformas",
  "precio": 12.5,
  "stock": 5
}
```

Resultado esperado: `201 Created`. La colección guarda el `id` recibido en `juegoId`.

El título se mantiene limpio como `Celeste`. Si repites el registro, la API responderá `409 Conflict` porque los títulos son únicos; cambia el título para crear otro juego.

#### 2. Consultar catálogo

Envía `GET /juegos`.

Resultado esperado: `200 OK` y un arreglo con los juegos almacenados.

#### 3. Actualizar videojuego

Envía `PATCH /juegos/{{juegoId}}` con:

```json
{
  "precio": 10,
  "stock": 8
}
```

Resultado esperado: `200 OK`. Solo cambian los campos enviados.

#### 4. Comprar videojuego

Envía `POST /juegos/{{juegoId}}/compras` sin body.

Resultado esperado: `201 Created`. La respuesta contiene la venta y el stock del juego baja de 8 a 7.

Ejecuta otra vez **Consultar catálogo** para demostrar el descuento de stock.

## 6. Probar desde el archivo `.http`

También puedes abrir:

```text
src/GameStore.Api/GameStore.Api.http
```

Visual Studio y extensiones como REST Client muestran el botón **Send Request** encima de cada petición. Después de registrar un juego, copia su `id` y reemplaza `reemplazar-con-id` en la variable `juegoId`.

## 7. Ejecutar pruebas unitarias

Las pruebas unitarias no necesitan Docker ni PostgreSQL:

```powershell
dotnet test tests/unit/GameStore.UnitTests/GameStore.UnitTests.csproj
```

Comprueban:

- Creación de eventos de dominio.
- Rechazo de compras sin stock.
- Actualización parcial.
- Creación de una venta y descuento del stock.

Resultado esperado: cuatro pruebas aprobadas.

## 8. Ejecutar pruebas funcionales con Aspire

Docker Desktop debe estar encendido. Detén antes cualquier ejecución manual de `aspire start` para evitar conflictos de puertos.

Ejecuta:

```powershell
dotnet test tests/integration/GameStore.FunctionalTests/GameStore.FunctionalTests.csproj
```

La prueba usa `Aspire.Hosting.Testing` y hace automáticamente lo siguiente:

1. Construye `GameStore.AppHost`.
2. Levanta PostgreSQL.
3. Inicia la API real.
4. Espera el estado saludable.
5. Registra, actualiza, compra y consulta por HTTP.
6. Verifica los códigos y el stock final.

Para ejecutar todas las pruebas:

```powershell
dotnet test GameStore.slnx
```

## 9. Explicar la arquitectura al profesor

Recorre los proyectos en este orden:

1. `GameStore.Domain`: enseña `Juego`, `Venta`, `TituloJuego`, `Dinero` y los eventos.
2. `GameStore.Application`: enseña los Commands, Query y Handlers por feature.
3. `GameStore.Infrastructure`: enseña `GameStoreDbContext`, configuraciones, repositorio y migraciones.
4. `GameStore.Api`: enseña el controlador y el manejador de errores.
5. `GameStore.AppHost`: enseña cómo se declaran PostgreSQL, la base y la API.
6. `GameStore.FunctionalTests`: enseña `DistributedApplicationTestingBuilder`.

Puntos importantes para mencionar:

- El dominio no depende de EF Core, ASP.NET ni Aspire.
- Las propiedades tienen setters privados.
- Las ventas son de solo lectura desde afuera del agregado.
- Los Commands escriben y la Query lee.
- Infraestructura implementa `IRepositorioJuegos` definido en Application.
- Aspire administra infraestructura, configuración, health checks y observabilidad.

## 10. Errores comunes

### Docker no está ejecutándose

Síntoma: Aspire no puede crear `postgres`.

Solución: abre Docker Desktop, espera a que finalice y repite `aspire start`.

### Puerto 3000 ocupado

Síntoma: `api` no inicia porque otro proceso usa el puerto.

Solución: detén el backend anterior o cualquier otra aplicación que use `3000`, y vuelve a iniciar Aspire.

### `409 Conflict` al crear

El título ya existe. Usa otro título, por ejemplo `Celeste 2`, o conserva el registro existente para continuar la demostración.

### `409 Conflict` al comprar

El juego no tiene stock. Actualízalo con `PATCH` y vuelve a comprar.

### `404 Not Found`

El `juegoId` no corresponde a un juego. Ejecuta primero la solicitud de registro para actualizar la variable.

### La API no está Healthy

Abre el recurso `api` en el dashboard y revisa sus logs. Normalmente el problema es PostgreSQL no disponible, Docker apagado o un puerto ocupado.
