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

`DATABASE_URL` será usada por la aplicación y `DIRECT_URL` por Prisma para migraciones.

## Estructura prevista

```text
src/
  domain/
    entities/
    repositories/
  application/
    use-cases/
  infrastructure/
    database/
    repositories/
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

1. El cliente envía `POST /games/:gameId/purchases`.
2. El controlador valida la solicitud y llama al caso de uso.
3. El caso de uso verifica que el juego exista y tenga stock.
4. El repositorio registra la venta y reduce el stock dentro de una transacción.
5. La API responde con `201 Created` o un error HTTP adecuado.

## Pendiente de implementación

1. Crear el proyecto de Supabase y completar `.env` con sus cadenas de conexión.
2. Ejecutar la primera migración de Prisma.
3. Importar la colección Postman y ejecutar sus solicitudes.

## Bitácora de avances

### 2026-07-18 — API inicial

- Se configuró el proyecto con NestJS y TypeScript.
- Se separaron dominio, aplicación, infraestructura y presentación.
- Se implementaron los endpoints para registrar, consultar y comprar videojuegos.
- Se preparó el esquema de Prisma para Supabase PostgreSQL.
- Se mantendrán las pruebas y la colección Postman en commits posteriores.
