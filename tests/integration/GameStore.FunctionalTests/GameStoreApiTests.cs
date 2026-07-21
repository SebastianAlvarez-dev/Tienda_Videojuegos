using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace GameStore.FunctionalTests;

public sealed class GameStoreApiTests
{
    private static readonly TimeSpan TiempoMaximo = TimeSpan.FromMinutes(2);

    [Fact]
    public async Task Flujo_completo_funciona_sobre_apphost_y_postgresql()
    {
        var cancellationToken = CancellationToken.None;
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.GameStore_AppHost>(cancellationToken);

        appHost.Services.AddLogging(logging => logging.SetMinimumLevel(LogLevel.Warning));

        await using var app = await appHost.BuildAsync(cancellationToken)
            .WaitAsync(TiempoMaximo, cancellationToken);
        await app.StartAsync(cancellationToken).WaitAsync(TiempoMaximo, cancellationToken);
        await app.ResourceNotifications.WaitForResourceHealthyAsync("api", cancellationToken)
            .WaitAsync(TiempoMaximo, cancellationToken);

        using var client = app.CreateHttpClient("api", "http");
        var titulo = $"Juego E2E {Guid.NewGuid():N}";

        var crearRespuesta = await client.PostAsJsonAsync("/juegos", new
        {
            titulo,
            genero = "Pruebas",
            precio = 12.50m,
            stock = 5
        }, cancellationToken);
        Assert.Equal(HttpStatusCode.Created, crearRespuesta.StatusCode);
        var creado = await crearRespuesta.Content.ReadFromJsonAsync<JuegoApi>(cancellationToken);
        Assert.NotNull(creado);

        var actualizarRespuesta = await client.PatchAsJsonAsync($"/juegos/{creado.Id}", new
        {
            precio = 10m,
            stock = 8
        }, cancellationToken);
        Assert.Equal(HttpStatusCode.OK, actualizarRespuesta.StatusCode);

        var compraRespuesta = await client.PostAsync($"/juegos/{creado.Id}/compras", null, cancellationToken);
        Assert.Equal(HttpStatusCode.Created, compraRespuesta.StatusCode);

        var juegos = await client.GetFromJsonAsync<JuegoApi[]>("/juegos", cancellationToken);
        var juego = Assert.Single(juegos!, item => item.Id == creado.Id);
        Assert.Equal(7, juego.Stock);
        Assert.Equal(10, juego.Precio);
    }

    private sealed record JuegoApi(Guid Id, string Titulo, string Genero, decimal Precio, int Stock);
}
