using GameStore.Domain.Common;

namespace GameStore.Application.Abstracciones;

public interface IManejadorEventoDominio
{
    Type TipoEvento { get; }
    Task ManejarAsync(IEventoDominio eventoDominio, CancellationToken cancellationToken);
}

public interface IDespachadorEventosDominio
{
    Task DespacharAsync(IEnumerable<IEventoDominio> eventosDominio, CancellationToken cancellationToken);
}
