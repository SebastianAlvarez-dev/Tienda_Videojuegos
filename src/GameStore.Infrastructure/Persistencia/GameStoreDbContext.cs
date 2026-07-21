using GameStore.Application.Abstracciones;
using GameStore.Domain.Common;
using GameStore.Domain.Entidades;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.Persistencia;

public sealed class GameStoreDbContext(
    DbContextOptions<GameStoreDbContext> options,
    IDespachadorEventosDominio despachadorEventos) : DbContext(options)
{
    public DbSet<Juego> Juegos => Set<Juego>();
    public DbSet<Venta> Ventas => Set<Venta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GameStoreDbContext).Assembly);

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entidades = ChangeTracker.Entries<Entidad>()
            .Where(entry => entry.Entity.EventosDominio.Count > 0)
            .Select(entry => entry.Entity)
            .ToArray();

        var resultado = await base.SaveChangesAsync(cancellationToken);
        var eventos = entidades.SelectMany(entidad => entidad.ExtraerEventosDominio()).ToArray();
        await despachadorEventos.DespacharAsync(eventos, cancellationToken);
        return resultado;
    }
}
