using GameStore.Application.Abstracciones;
using GameStore.Application.Contratos;

namespace GameStore.Application.Features.Juegos.ListarJuegos;

public sealed record ListarJuegosQuery : IConsulta<IReadOnlyList<JuegoRespuesta>>;
