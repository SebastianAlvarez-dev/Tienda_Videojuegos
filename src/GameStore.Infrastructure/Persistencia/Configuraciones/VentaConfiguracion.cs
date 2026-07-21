using GameStore.Domain.Entidades;
using GameStore.Domain.ObjetosValor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Infrastructure.Persistencia.Configuraciones;

public sealed class VentaConfiguracion : IEntityTypeConfiguration<Venta>
{
    public void Configure(EntityTypeBuilder<Venta> builder)
    {
        builder.ToTable("ventas");
        builder.HasKey(venta => venta.Id);
        builder.Property(venta => venta.Id).HasColumnName("id");
        builder.Property(venta => venta.JuegoId).HasColumnName("juego_id");
        builder.Property(venta => venta.Precio)
            .HasConversion(precio => precio.Valor, valor => Dinero.Crear(valor))
            .HasColumnName("precio")
            .HasPrecision(10, 2)
            .IsRequired();
        builder.Property(venta => venta.FechaCreacion).HasColumnName("fecha_creacion");
        builder.Ignore(venta => venta.EventosDominio);
        builder.HasIndex(venta => venta.JuegoId);
    }
}
