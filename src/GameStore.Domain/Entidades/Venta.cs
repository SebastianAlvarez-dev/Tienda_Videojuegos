using GameStore.Domain.Common;
using GameStore.Domain.ObjetosValor;

namespace GameStore.Domain.Entidades;

public sealed class Venta : Entidad
{
    private Venta() { }

    private Venta(Guid id, Guid juegoId, Dinero precio)
    {
        Id = id;
        JuegoId = juegoId;
        Precio = precio;
        FechaCreacion = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid JuegoId { get; private set; }
    public Dinero Precio { get; private set; } = null!;
    public DateTimeOffset FechaCreacion { get; private set; }

    internal static Venta Crear(Guid juegoId, Dinero precio) => new(Guid.NewGuid(), juegoId, precio);
}
