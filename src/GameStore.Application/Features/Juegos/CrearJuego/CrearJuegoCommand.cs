using GameStore.Application.Abstracciones;
using GameStore.Application.Contratos;

namespace GameStore.Application.Features.Juegos.CrearJuego;

public sealed record CrearJuegoCommand(string Titulo, string Genero, decimal Precio, int Stock)
    : IComando<JuegoRespuesta>;
