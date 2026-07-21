using GameStore.Application.Abstracciones;
using GameStore.Application.Contratos;
using GameStore.Domain.Entidades;
using GameStore.Domain.Excepciones;
using GameStore.Domain.ObjetosValor;

namespace GameStore.Application.Features.Juegos.CrearJuego;

public sealed class CrearJuegoHandler(IRepositorioJuegos repositorio)
    : IManejadorComando<CrearJuegoCommand, JuegoRespuesta>
{
    public async Task<JuegoRespuesta> ManejarAsync(CrearJuegoCommand comando, CancellationToken cancellationToken)
    {
        var titulo = TituloJuego.Crear(comando.Titulo);
        if (await repositorio.ExisteTituloAsync(titulo, null, cancellationToken))
        {
            throw new ExcepcionDominio("JUEGO_YA_EXISTE", "Ya existe un videojuego con ese título.");
        }

        var juego = Juego.Crear(titulo, comando.Genero, Dinero.Crear(comando.Precio), comando.Stock);
        await repositorio.AgregarAsync(juego, cancellationToken);
        await repositorio.GuardarCambiosAsync(cancellationToken);
        return JuegoRespuesta.Desde(juego);
    }
}
