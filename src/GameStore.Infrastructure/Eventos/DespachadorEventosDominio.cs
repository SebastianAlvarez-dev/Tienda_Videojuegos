using GameStore.Application.Abstracciones;
using GameStore.Domain.Common;

namespace GameStore.Infrastructure.Eventos;

public sealed class DespachadorEventosDominio(IEnumerable<IManejadorEventoDominio> manejadores)
    : IDespachadorEventosDominio
{
    public async Task DespacharAsync(
        IEnumerable<IEventoDominio> eventosDominio,
        CancellationToken cancellationToken)
    {
        foreach (var evento in eventosDominio)
        {
            foreach (var manejador in manejadores.Where(item => item.TipoEvento == evento.GetType()))
            {
                await manejador.ManejarAsync(evento, cancellationToken);
            }
        }
    }
}
