using GameStore.Domain.Entidades;
using GameStore.Domain.ObjetosValor;

namespace GameStore.Application.Abstracciones;

public interface IRepositorioJuegos
{
    Task AgregarAsync(Juego juego, CancellationToken cancellationToken);
    Task AgregarVentaAsync(Venta venta, CancellationToken cancellationToken);
    Task<Juego?> ObtenerPorIdAsync(Guid juegoId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Juego>> ListarAsync(CancellationToken cancellationToken);
    Task<bool> ExisteTituloAsync(TituloJuego titulo, Guid? excluirJuegoId, CancellationToken cancellationToken);
    Task GuardarCambiosAsync(CancellationToken cancellationToken);
}
