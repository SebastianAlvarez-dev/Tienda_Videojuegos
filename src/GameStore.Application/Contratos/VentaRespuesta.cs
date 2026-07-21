using GameStore.Domain.Entidades;

namespace GameStore.Application.Contratos;

public sealed record VentaRespuesta(
    Guid Id,
    Guid JuegoId,
    decimal Precio,
    DateTimeOffset FechaCreacion)
{
    public static VentaRespuesta Desde(Venta venta) =>
        new(venta.Id, venta.JuegoId, venta.Precio.Valor, venta.FechaCreacion);
}
