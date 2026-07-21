using GameStore.Application.Abstracciones;
using GameStore.Domain.Common;
using GameStore.Domain.Eventos;
using Microsoft.Extensions.Logging;

namespace GameStore.Infrastructure.Eventos;

public sealed class RegistrarCompraRealizada(ILogger<RegistrarCompraRealizada> logger)
    : IManejadorEventoDominio
{
    public Type TipoEvento => typeof(CompraRealizada);

    public Task ManejarAsync(IEventoDominio eventoDominio, CancellationToken cancellationToken)
    {
        var compra = (CompraRealizada)eventoDominio;
        logger.LogInformation(
            "Compra {VentaId} registrada para el juego {JuegoId} por {Precio}",
            compra.VentaId,
            compra.JuegoId,
            compra.Precio);
        return Task.CompletedTask;
    }
}
