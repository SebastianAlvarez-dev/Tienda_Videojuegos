using GameStore.Application.Abstracciones;
using GameStore.Domain.Entidades;
using GameStore.Domain.Excepciones;
using GameStore.Domain.ObjetosValor;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace GameStore.Infrastructure.Persistencia;

public sealed class RepositorioJuegos(GameStoreDbContext dbContext) : IRepositorioJuegos
{
    public async Task AgregarAsync(Juego juego, CancellationToken cancellationToken) =>
        await dbContext.Juegos.AddAsync(juego, cancellationToken);

    public async Task AgregarVentaAsync(Venta venta, CancellationToken cancellationToken) =>
        await dbContext.Ventas.AddAsync(venta, cancellationToken);

    public Task<Juego?> ObtenerPorIdAsync(Guid juegoId, CancellationToken cancellationToken) =>
        dbContext.Juegos.SingleOrDefaultAsync(juego => juego.Id == juegoId, cancellationToken);

    public async Task<IReadOnlyList<Juego>> ListarAsync(CancellationToken cancellationToken) =>
        await dbContext.Juegos.AsNoTracking()
            .OrderByDescending(juego => juego.FechaCreacion)
            .ToListAsync(cancellationToken);

    public Task<bool> ExisteTituloAsync(
        TituloJuego titulo,
        Guid? excluirJuegoId,
        CancellationToken cancellationToken) =>
        dbContext.Juegos.AnyAsync(
            juego => juego.Titulo == titulo && (!excluirJuegoId.HasValue || juego.Id != excluirJuegoId.Value),
            cancellationToken);

    public async Task GuardarCambiosAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new ExcepcionDominio("JUEGO_YA_EXISTE", "Ya existe un videojuego con ese título.");
        }
    }
}
