using GameStore.Application.Abstracciones;
using GameStore.Application.Contratos;

namespace GameStore.Application.Features.Juegos.ActualizarJuego;

public sealed record ActualizarJuegoCommand(
    Guid JuegoId,
    string? Titulo,
    string? Genero,
    decimal? Precio,
    int? Stock) : IComando<JuegoRespuesta>;
