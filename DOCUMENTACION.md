# Documentación técnica

## Requisitos locales

- Node.js LTS.
- Visual Studio Code.
- Cuenta y proyecto de Supabase.
- Postman.

## Variables de entorno previstas

Crear un archivo `.env` a partir de `.env.example` cuando se cree el proyecto:

```env
DATABASE_URL="conexion-pooler-de-supabase"
DIRECT_URL="conexion-directa-de-supabase"
PORT=3000
```

`DATABASE_URL` será usada por la aplicación y `DIRECT_URL` por Prisma para migraciones. En desarrollo local se puede usar la misma cadena de **Session pooler** (puerto `5432`) en ambas variables.

## Estructura del proyecto

```text
src/
  domain/
    entities/
    errors/
    ports/
  application/
    use-cases/
  infrastructure/
    prisma-game.repository.ts
  presentation/
    http/
test/
  unit/
  integration/
postman/
prisma/
```

## Ejecución

```bash
npm run start:dev
npm run test
npm run test:unit
npm run test:integration
npm run prisma:generate
npm run prisma:migrate -- --name init
```

Se ejecutan desde la terminal integrada de Visual Studio Code. La extensión **Vitest** permite ver y ejecutar los archivos `test/**/*.spec.ts` desde el panel **Testing**.

## Convenciones HTTP

| Situación | Código |
| --- | --- |
| Recurso creado | `201 Created` |
| Consulta correcta | `200 OK` |
| Datos inválidos | `400 Bad Request` |
| Recurso no encontrado | `404 Not Found` |
| Stock insuficiente | `409 Conflict` |

## Pruebas

- **Unitarias:** prueban las reglas y casos de uso con repositorios simulados, sin Supabase.
- **Integración:** prueban los endpoints HTTP con la aplicación NestJS.
- **Manual:** la colección `postman/GameStore.postman_collection.json` tendrá solicitudes para cada endpoint.

## Flujo de una compra

1. El cliente envía `POST /juegos/:juegoId/compras`.
2. El controlador valida la solicitud y llama al caso de uso.
3. El caso de uso verifica que el juego exista y tenga stock.
4. El repositorio registra la venta y reduce el stock dentro de una transacción.
5. La API responde con `201 Created` o un error HTTP adecuado.

## Bitácora de avances

### 2026-07-18 — API inicial

- Se configuró el proyecto con NestJS y TypeScript.
- Se separaron dominio, aplicación, infraestructura y presentación.
- Se implementaron los endpoints para registrar, consultar y comprar videojuegos.
- Se preparó el esquema de Prisma para Supabase PostgreSQL.
- Se mantendrán las pruebas y la colección Postman en commits posteriores.

### 2026-07-18 — Base de datos Supabase

- Se creó el proyecto `gamestore-db` en la región `us-east-1`.
- Se crearon las tablas `Juego` y `Venta` con sus claves, relación, índice y restricciones de precio y stock.
- RLS está activo y no hay políticas públicas, porque la base se usa únicamente desde el backend con Prisma.

### 2026-07-18 — Prueba real de la API

- Se registró la migración inicial en Prisma sin exponer la contraseña de la base.
- Se verificaron los tres casos de uso reales contra Supabase: crear un juego, consultar el catálogo y registrar una compra.
- La API se ejecuta localmente en el puerto `3000` con las rutas `/juegos` y `/juegos/:juegoId/compras`.

### 2026-07-18 — API y pruebas en español

- Se tradujeron las rutas, los cuerpos JSON, los errores y el modelo de datos a español.
- Se añadieron pruebas unitarias y de integración con Vitest.
- Se agregó una colección Postman para registrar, consultar y comprar videojuegos.
- Se creó `README.md` con los pasos de instalación, uso y pruebas del proyecto.

### 2026-07-18 — Organización para explicación académica

- Se separaron las entidades `Juego` y `Venta`, el error de dominio y el puerto del repositorio.
- Cada caso de uso tiene su propio archivo: crear, listar y comprar juegos.
- La estructura facilita explicar las responsabilidades de las capas de Clean Architecture.

## Guía práctica de prueba

### 1. Configurar Supabase

1. Crear un proyecto en Supabase.
2. Copiar `.env.example` como `.env`.
3. En Supabase, abrir **Connect** y copiar la conexión de pooler en `DATABASE_URL` y la conexión directa en `DIRECT_URL`.

No se usa una clave pública de Supabase: Prisma se conecta directamente a PostgreSQL mediante estas cadenas.

### 2. Preparar y ejecutar la API

```powershell
npm install
Copy-Item .env.example .env
# Completar DATABASE_URL y DIRECT_URL en .env
npm run prisma:generate
npm run start:dev
```

La API queda disponible en `http://localhost:3000`.

La primera migración ya fue aplicada en Supabase. Para registrar ese estado en Prisma, ejecutar una única vez:

```powershell
npx prisma migrate resolve --applied 20260718040000_create_gamestore_tables
```

### 3. Endpoints REST

| Método | Ruta | Cuerpo | Resultado |
| --- | --- | --- | --- |
| `POST` | `/juegos` | `titulo`, `genero`, `precio`, `stock` | Crea un videojuego (`201`) |
| `GET` | `/juegos` | No requiere | Lista el catálogo (`200`) |
| `PATCH` | `/juegos/:juegoId` | Uno o más campos del juego | Actualiza el juego (`200`) |
| `POST` | `/juegos/:juegoId/compras` | No requiere | Registra la compra (`201`) |

Ejemplo para crear un juego:

```json
{
  "titulo": "Celeste",
  "genero": "Plataformas",
  "precio": 12.5,
  "stock": 5
}
```

Errores esperados: `400` para datos inválidos, `404` si el juego no existe y `409` si ya existe el título o no queda stock.

### 4. Postman

1. Abrir Postman y elegir **Import**.
2. Seleccionar `postman/GameStore.postman_collection.json`.
3. Ejecutar **Registrar videojuego**; la colección guarda automáticamente el identificador recibido.
4. Ejecutar **Consultar catálogo**.
5. Ejecutar **Comprar videojuego**; utiliza el identificador guardado en la variable `juegoId`.

La variable `baseUrl` inicia con el valor `http://localhost:3000` y puede cambiarse desde las variables de la colección.

### 2026-07-18 — Actualización de videojuegos

- Se agregó el caso de uso para actualizar título, género, precio o stock de forma parcial.
- La API expone `PATCH /juegos/:juegoId` y conserva las ventas registradas.
