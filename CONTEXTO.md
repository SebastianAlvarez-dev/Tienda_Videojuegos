# Contexto del sistema

## Proyecto

**GameStore API** es una API REST para una tienda digital de videojuegos. Permite administrar el catálogo y registrar compras de juegos con control de stock.

## Objetivo académico

Demostrar una aplicación de servidor con:

- Clean Architecture y DDD ligero.
- Inyección de dependencias (DI) mediante NestJS.
- API REST con métodos HTTP y códigos de respuesta correctos.
- Persistencia en Supabase PostgreSQL mediante Prisma ORM.
- Pruebas unitarias e integración ejecutables desde Visual Studio Code.
- Colección Postman para probar los endpoints.

## Alcance inicial

No se implementarán autenticación, pagos reales ni frontend. Una compra es un registro interno; su objetivo es demostrar reglas de negocio, transacciones y persistencia.

## Casos de uso

1. **Registrar videojuego:** crear un juego con título, precio, stock y género.
2. **Consultar catálogo:** listar los videojuegos registrados.
3. **Registrar compra:** descontar una unidad del stock y guardar la venta.

## Reglas de negocio

- El título de un juego es obligatorio.
- El precio debe ser mayor que cero.
- El stock inicial no puede ser negativo.
- No se puede comprar un juego inexistente ni sin stock.
- Una compra debe actualizar el stock y crear la venta en una única transacción.

## Arquitectura

```text
HTTP / NestJS controllers
          |
          v
Application (casos de uso)
          |
          v
Domain (entidades, reglas y puertos)
          ^
          |
Infrastructure (Prisma, Supabase y repositorios)
```

- **Domain:** no depende de NestJS, Prisma ni Supabase.
- **Application:** orquesta los casos de uso y depende de puertos del dominio.
- **Infrastructure:** implementa los puertos usando Prisma y Supabase PostgreSQL.
- **Presentation:** controladores NestJS, DTOs y manejo HTTP.

## Stack aprobado

| Área | Tecnología |
| --- | --- |
| Runtime y lenguaje | Node.js + TypeScript |
| Framework API | NestJS |
| Base de datos | Supabase PostgreSQL |
| ORM | Prisma |
| Validación HTTP | class-validator y class-transformer |
| Pruebas | Vitest + Supertest |
| Cliente de API | Postman |
| Documentación | Markdown |

## Recursos principales

- `Juego`: videojuego disponible para la venta.
- `Venta`: registro de una compra realizada.

## Endpoints previstos

| Método | Ruta | Caso de uso | Respuesta exitosa |
| --- | --- | --- | --- |
| `POST` | `/juegos` | Registrar videojuego | `201 Created` |
| `GET` | `/juegos` | Consultar catálogo | `200 OK` |
| `PATCH` | `/juegos/:juegoId` | Actualizar videojuego | `200 OK` |
| `POST` | `/juegos/:juegoId/compras` | Registrar compra | `201 Created` |

## Criterios de aceptación

- Los tres casos de uso funcionan a través de la API.
- Las reglas de negocio tienen pruebas unitarias.
- Los endpoints tienen pruebas de integración.
- La colección de Postman permite ejecutar las tres operaciones.
- Los comandos de prueba funcionan en la terminal integrada de Visual Studio Code.
