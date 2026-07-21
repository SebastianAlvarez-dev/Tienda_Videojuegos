using GameStore.Domain.Excepciones;

namespace GameStore.Domain.ObjetosValor;

public sealed record Dinero
{
    private Dinero(decimal valor) => Valor = valor;

    public decimal Valor { get; }

    public static Dinero Crear(decimal valor)
    {
        if (valor <= 0)
        {
            throw new ExcepcionDominio("JUEGO_INVALIDO", "El precio debe ser mayor que cero.");
        }

        return new Dinero(decimal.Round(valor, 2));
    }

    public override string ToString() => Valor.ToString("0.00");
}
