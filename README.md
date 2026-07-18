# GameStore API

API REST para administrar una tienda de videojuegos. Implementa Clean Architecture, NestJS, Prisma y Supabase PostgreSQL.

## Funcionalidades

- Registrar un videojuego.
- Consultar el catálogo.
- Registrar una compra y descontar el stock.

## Tecnologías

- Node.js y TypeScript.
- NestJS.
- Prisma ORM y Supabase PostgreSQL.
- Vitest y Supertest.
- Postman.

## Configuración

1. Instalar dependencias:

   ```powershell
   npm install
   ```

2. Copiar `.env.example` a `.env` y completar las dos URLs de PostgreSQL de Supabase. No subir `.env` al repositorio.

3. Generar Prisma y aplicar las migraciones en una base nueva:

   ```powershell
   npm run prisma:generate
   npx prisma migrate deploy
   ```

4. Iniciar la API:

   ```powershell
   npm run start:dev
   ```

La API queda disponible en `http://localhost:3000`.

## Endpoints

| Método | Ruta | Descripción |
| --- | --- | --- |
| `POST` | `/juegos` | Crea un videojuego. |
| `GET` | `/juegos` | Lista el catálogo. |
| `POST` | `/juegos/:juegoId/compras` | Registra una compra. |

Ejemplo para crear un juego:

```json
{
  "titulo": "Celeste",
  "genero": "Plataformas",
  "precio": 12.5,
  "stock": 5
}
```

## Pruebas

```powershell
npm test
npm run test:unit
npm run test:integration
```

## Postman

Importar `postman/GameStore.postman_collection.json` o crear las tres solicitudes con las rutas indicadas. Ejecutarlas en el orden: registrar, consultar y comprar.

## Arquitectura

```text
src/domain           Entidades y puertos
  entities/          Juego y Venta
  errors/            Errores de negocio
  ports/             Contratos de repositorio
src/application
  use-cases/         Un archivo por caso de uso
src/infrastructure   Prisma y repositorios
src/presentation     Controladores HTTP y validación
```

La documentación ampliada y la bitácora están en `CONTEXTO.md` y `DOCUMENTACION.md`.
