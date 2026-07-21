using GameStore.Application.Abstracciones;
using GameStore.Application.Features.Compras.RegistrarCompra;
using GameStore.Application.Features.Juegos.ActualizarJuego;
using GameStore.Application.Features.Juegos.CrearJuego;
using GameStore.Domain.Entidades;
using GameStore.Domain.Eventos;
using GameStore.Domain.Excepciones;
using GameStore.Domain.ObjetosValor;

namespace GameStore.UnitTests;

public sealed class JuegosTests
{
    [Fact]
    public void Crear_juego_registra_evento_de_dominio()
    {
        var juego = Juego.Crear(
            TituloJuego.Crear("Celeste"),
            "Plataformas",
            Dinero.Crear(12.50m),
            5);

        Assert.Contains(juego.EventosDominio, evento => evento is JuegoCreado);
    }

    [Fact]
    public void Comprar_sin_stock_es_rechazado()
    {
        var juego = Juego.Crear(
            TituloJuego.Crear("Hades"),
            "Acción",
            Dinero.Crear(20),
            0);

        var error = Assert.Throws<ExcepcionDominio>(() => juego.Comprar());

        Assert.Equal("STOCK_INSUFICIENTE", error.Codigo);
    }

    [Fact]
    public async Task Actualizar_solo_precio_conserva_los_demas_datos()
    {
        var repositorio = new RepositorioJuegosFalso();
        var creado = await new CrearJuegoHandler(repositorio).ManejarAsync(
            new CrearJuegoCommand("Hollow Knight", "Metroidvania", 15, 4),
            CancellationToken.None);

        var actualizado = await new ActualizarJuegoHandler(repositorio).ManejarAsync(
            new ActualizarJuegoCommand(creado.Id, null, null, 10, null),
            CancellationToken.None);

        Assert.Equal("Hollow Knight", actualizado.Titulo);
        Assert.Equal(10, actualizado.Precio);
        Assert.Equal(4, actualizado.Stock);
    }

    [Fact]
    public async Task Comprar_descuenta_stock_y_crea_venta()
    {
        var repositorio = new RepositorioJuegosFalso();
        var creado = await new CrearJuegoHandler(repositorio).ManejarAsync(
            new CrearJuegoCommand("Ori", "Metroidvania", 18, 2),
            CancellationToken.None);

        var venta = await new RegistrarCompraHandler(repositorio).ManejarAsync(
            new RegistrarCompraCommand(creado.Id),
            CancellationToken.None);
        var juego = await repositorio.ObtenerPorIdAsync(creado.Id, CancellationToken.None);

        Assert.Equal(creado.Id, venta.JuegoId);
        Assert.Equal(1, juego!.Stock);
        Assert.Contains(juego.EventosDominio, evento => evento is CompraRealizada);
    }

    private sealed class RepositorioJuegosFalso : IRepositorioJuegos
    {
        private readonly List<Juego> _juegos = [];

        public Task AgregarAsync(Juego juego, CancellationToken cancellationToken)
        {
            _juegos.Add(juego);
            return Task.CompletedTask;
        }

        public Task AgregarVentaAsync(Venta venta, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task<Juego?> ObtenerPorIdAsync(Guid juegoId, CancellationToken cancellationToken) =>
            Task.FromResult(_juegos.SingleOrDefault(juego => juego.Id == juegoId));

        public Task<IReadOnlyList<Juego>> ListarAsync(CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<Juego>>(_juegos);

        public Task<bool> ExisteTituloAsync(
            TituloJuego titulo,
            Guid? excluirJuegoId,
            CancellationToken cancellationToken) =>
            Task.FromResult(_juegos.Any(juego =>
                juego.Titulo == titulo && (!excluirJuegoId.HasValue || juego.Id != excluirJuegoId.Value)));

        public Task GuardarCambiosAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
