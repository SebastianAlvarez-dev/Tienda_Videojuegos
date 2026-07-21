using GameStore.Domain.Entidades;
using GameStore.Domain.ObjetosValor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Infrastructure.Persistencia.Configuraciones;

public sealed class JuegoConfiguracion : IEntityTypeConfiguration<Juego>
{
    public void Configure(EntityTypeBuilder<Juego> builder)
    {
        builder.ToTable("juegos");
        builder.HasKey(juego => juego.Id);
        builder.Property(juego => juego.Id).HasColumnName("id");
        builder.Property(juego => juego.Titulo)
            .HasConversion(titulo => titulo.Valor, valor => TituloJuego.Crear(valor))
            .HasColumnName("titulo")
            .HasMaxLength(150)
            .IsRequired();
        builder.HasIndex(juego => juego.Titulo).IsUnique();
        builder.Property(juego => juego.Genero)
            .HasColumnName("genero")
            .HasMaxLength(80)
            .IsRequired();
        builder.Property(juego => juego.Precio)
            .HasConversion(precio => precio.Valor, valor => Dinero.Crear(valor))
            .HasColumnName("precio")
            .HasPrecision(10, 2)
            .IsRequired();
        builder.Property(juego => juego.Stock).HasColumnName("stock");
        builder.Property(juego => juego.FechaCreacion).HasColumnName("fecha_creacion");
        builder.Ignore(juego => juego.EventosDominio);
        builder.HasMany(juego => juego.Ventas)
            .WithOne()
            .HasForeignKey(venta => venta.JuegoId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(juego => juego.Ventas).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
