using GameStore.Application.Abstracciones;
using GameStore.Application.Contratos;
using GameStore.Domain.Excepciones;

namespace GameStore.Application.Features.Compras.RegistrarCompra;

public sealed class RegistrarCompraHandler(IRepositorioJuegos repositorio)
    : IManejadorComando<RegistrarCompraCommand, VentaRespuesta>
{
    public async Task<VentaRespuesta> ManejarAsync(
        RegistrarCompraCommand comando,
        CancellationToken cancellationToken)
    {
        var juego = await repositorio.ObtenerPorIdAsync(comando.JuegoId, cancellationToken)
            ?? throw new ExcepcionDominio("JUEGO_NO_ENCONTRADO", "El videojuego no existe.");

        var venta = juego.Comprar();
        await repositorio.AgregarVentaAsync(venta, cancellationToken);
        await repositorio.GuardarCambiosAsync(cancellationToken);
        return VentaRespuesta.Desde(venta);
    }
}
