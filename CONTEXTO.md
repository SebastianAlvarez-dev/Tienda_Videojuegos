# Contexto del sistema

## Propósito

GameStore administra el catálogo y las ventas de una tienda digital de videojuegos. No incluye frontend, pagos reales, usuarios ni autenticación: su alcance está enfocado en demostrar reglas de negocio, arquitectura empresarial y pruebas automatizadas.

## Lenguaje ubicuo

| Término | Significado |
| --- | --- |
| Juego | Videojuego disponible en el catálogo. |
| Catálogo | Conjunto de juegos registrados. |
| Stock | Unidades disponibles para comprar. |
| Venta | Registro inmutable de una compra realizada. |
| Precio | Valor monetario positivo con dos decimales. |

## Agregado principal

`Juego` es la raíz del agregado. Controla su título, género, precio, stock y colección de ventas. Nadie puede modificar directamente sus propiedades porque tienen setters privados.

`Venta` se crea únicamente mediante `Juego.Comprar()`. De esta forma, toda compra verifica el stock y descuenta una unidad antes de registrar la venta.

## Reglas de negocio

1. El título es obligatorio, único y admite máximo 150 caracteres.
2. El género es obligatorio y admite máximo 80 caracteres.
3. El precio debe ser mayor que cero y se guarda con dos decimales.
4. El stock nunca puede ser negativo.
5. Una actualización debe contener al menos un campo.
6. No puede comprarse un juego inexistente o sin stock.
7. La venta conserva el precio vigente al momento de la compra.

## Casos de uso

1. Registrar un videojuego.
2. Consultar el catálogo.
3. Actualizar parcialmente un videojuego.
4. Registrar una compra y descontar stock.

## Eventos de dominio

- `JuegoCreado`: se registra cuando nace un juego.
- `JuegoActualizado`: se registra al cambiar datos del catálogo.
- `CompraRealizada`: se registra después de una compra válida.

Las entidades acumulan eventos y `GameStoreDbContext` los despacha después de persistir correctamente los cambios. `RegistrarCompraRealizada` demuestra un consumidor desacoplado mediante logging estructurado.

## Decisiones de alcance

- PostgreSQL se ejecuta como contenedor administrado por Aspire para que el entorno sea reproducible durante la exposición.
- EF Core aplica las migraciones automáticamente al iniciar la API.
- CQRS se implementa con interfaces y handlers propios; no se agregó MediatR porque la rúbrica exige el patrón, no esa dependencia.
- Postman y `GameStore.Api.http` cubren todos los flujos evaluables.
