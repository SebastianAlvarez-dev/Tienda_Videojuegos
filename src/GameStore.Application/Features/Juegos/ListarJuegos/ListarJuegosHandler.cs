using GameStore.Application.Abstracciones;
using GameStore.Application.Contratos;

namespace GameStore.Application.Features.Juegos.ListarJuegos;

public sealed class ListarJuegosHandler(IRepositorioJuegos repositorio)
    : IManejadorConsulta<ListarJuegosQuery, IReadOnlyList<JuegoRespuesta>>
{
    public async Task<IReadOnlyList<JuegoRespuesta>> ManejarAsync(
        ListarJuegosQuery consulta,
        CancellationToken cancellationToken)
    {
        var juegos = await repositorio.ListarAsync(cancellationToken);
        return juegos.Select(JuegoRespuesta.Desde).ToArray();
    }
}
