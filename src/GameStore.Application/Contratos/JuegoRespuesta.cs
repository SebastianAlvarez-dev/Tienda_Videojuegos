using GameStore.Domain.Entidades;

namespace GameStore.Application.Contratos;

public sealed record JuegoRespuesta(
    Guid Id,
    string Titulo,
    string Genero,
    decimal Precio,
    int Stock,
    DateTimeOffset FechaCreacion)
{
    public static JuegoRespuesta Desde(Juego juego) => new(
        juego.Id,
        juego.Titulo.Valor,
        juego.Genero,
        juego.Precio.Valor,
        juego.Stock,
        juego.FechaCreacion);
}
