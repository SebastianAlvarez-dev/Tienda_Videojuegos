using GameStore.Domain.Common;
using GameStore.Domain.Eventos;
using GameStore.Domain.Excepciones;
using GameStore.Domain.ObjetosValor;

namespace GameStore.Domain.Entidades;

public sealed class Juego : Entidad
{
    private readonly List<Venta> _ventas = [];

    private Juego() { }

    private Juego(Guid id, TituloJuego titulo, string genero, Dinero precio, int stock)
    {
        Id = id;
        Titulo = titulo;
        Genero = ValidarGenero(genero);
        Precio = precio;
        Stock = ValidarStock(stock);
        FechaCreacion = DateTimeOffset.UtcNow;
        RegistrarEvento(new JuegoCreado(Id, Titulo.Valor, DateTimeOffset.UtcNow));
    }

    public Guid Id { get; private set; }
    public TituloJuego Titulo { get; private set; } = null!;
    public string Genero { get; private set; } = string.Empty;
    public Dinero Precio { get; private set; } = null!;
    public int Stock { get; private set; }
    public DateTimeOffset FechaCreacion { get; private set; }
    public IReadOnlyCollection<Venta> Ventas => _ventas.AsReadOnly();

    public static Juego Crear(TituloJuego titulo, string genero, Dinero precio, int stock) =>
        new(Guid.NewGuid(), titulo, genero, precio, stock);

    public void Actualizar(TituloJuego? titulo, string? genero, Dinero? precio, int? stock)
    {
        if (titulo is null && genero is null && precio is null && stock is null)
        {
            throw new ExcepcionDominio("JUEGO_INVALIDO", "Debe enviar al menos un campo para actualizar.");
        }

        if (titulo is not null) Titulo = titulo;
        if (genero is not null) Genero = ValidarGenero(genero);
        if (precio is not null) Precio = precio;
        if (stock is not null) Stock = ValidarStock(stock.Value);

        RegistrarEvento(new JuegoActualizado(Id, DateTimeOffset.UtcNow));
    }

    public Venta Comprar()
    {
        if (Stock <= 0)
        {
            throw new ExcepcionDominio("STOCK_INSUFICIENTE", "No existe stock disponible para este videojuego.");
        }

        Stock--;
        var venta = Venta.Crear(Id, Precio);
        _ventas.Add(venta);
        RegistrarEvento(new CompraRealizada(venta.Id, Id, venta.Precio.Valor, DateTimeOffset.UtcNow));
        return venta;
    }

    private static string ValidarGenero(string? genero)
    {
        var valor = genero?.Trim();
        if (string.IsNullOrWhiteSpace(valor) || valor.Length > 80)
        {
            throw new ExcepcionDominio("JUEGO_INVALIDO", "El género es obligatorio y admite máximo 80 caracteres.");
        }

        return valor;
    }

    private static int ValidarStock(int stock)
    {
        if (stock < 0)
        {
            throw new ExcepcionDominio("JUEGO_INVALIDO", "El stock no puede ser negativo.");
        }

        return stock;
    }
}
