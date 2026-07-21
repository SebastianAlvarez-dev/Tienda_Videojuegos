using GameStore.Application.Abstracciones;
using GameStore.Application.Contratos;
using GameStore.Domain.Excepciones;
using GameStore.Domain.ObjetosValor;

namespace GameStore.Application.Features.Juegos.ActualizarJuego;

public sealed class ActualizarJuegoHandler(IRepositorioJuegos repositorio)
    : IManejadorComando<ActualizarJuegoCommand, JuegoRespuesta>
{
    public async Task<JuegoRespuesta> ManejarAsync(
        ActualizarJuegoCommand comando,
        CancellationToken cancellationToken)
    {
        var juego = await repositorio.ObtenerPorIdAsync(comando.JuegoId, cancellationToken)
            ?? throw new ExcepcionDominio("JUEGO_NO_ENCONTRADO", "El videojuego no existe.");

        var titulo = comando.Titulo is null ? null : TituloJuego.Crear(comando.Titulo);
        if (titulo is not null && await repositorio.ExisteTituloAsync(titulo, juego.Id, cancellationToken))
        {
            throw new ExcepcionDominio("JUEGO_YA_EXISTE", "Ya existe un videojuego con ese título.");
        }

        juego.Actualizar(
            titulo,
            comando.Genero,
            comando.Precio is null ? null : Dinero.Crear(comando.Precio.Value),
            comando.Stock);

        await repositorio.GuardarCambiosAsync(cancellationToken);
        return JuegoRespuesta.Desde(juego);
    }
}
