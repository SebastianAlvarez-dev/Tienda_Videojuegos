using GameStore.Domain.Excepciones;

namespace GameStore.Domain.ObjetosValor;

public sealed record TituloJuego
{
    private TituloJuego(string valor) => Valor = valor;

    public string Valor { get; }

    public static TituloJuego Crear(string? valor)
    {
        var titulo = valor?.Trim();
        if (string.IsNullOrWhiteSpace(titulo) || titulo.Length > 150)
        {
            throw new ExcepcionDominio("JUEGO_INVALIDO", "El título es obligatorio y admite máximo 150 caracteres.");
        }

        return new TituloJuego(titulo);
    }

    public override string ToString() => Valor;
}
