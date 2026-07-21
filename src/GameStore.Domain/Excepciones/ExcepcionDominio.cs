namespace GameStore.Domain.Excepciones;

public sealed class ExcepcionDominio(string codigo, string message) : Exception(message)
{
    public string Codigo { get; } = codigo;
}
