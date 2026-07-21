using GameStore.Api.Errores;
using GameStore.Application.Abstracciones;
using GameStore.Application.Features.Compras.RegistrarCompra;
using GameStore.Application.Features.Juegos.ActualizarJuego;
using GameStore.Application.Features.Juegos.CrearJuego;
using GameStore.Application.Features.Juegos.ListarJuegos;
using GameStore.Infrastructure.Eventos;
using GameStore.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ManejadorExcepciones>();

var connectionString = builder.Configuration.GetConnectionString("gamestore")
    ?? throw new InvalidOperationException("Falta la conexión 'gamestore'. Inicie el proyecto mediante GameStore.AppHost.");

builder.Services.AddDbContext<GameStoreDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<IRepositorioJuegos, RepositorioJuegos>();
builder.Services.AddScoped<IDespachadorEventosDominio, DespachadorEventosDominio>();
builder.Services.AddScoped<IManejadorEventoDominio, RegistrarCompraRealizada>();
builder.Services.AddScoped<CrearJuegoHandler>();
builder.Services.AddScoped<ListarJuegosHandler>();
builder.Services.AddScoped<ActualizarJuegoHandler>();
builder.Services.AddScoped<RegistrarCompraHandler>();

var app = builder.Build();

app.UseExceptionHandler();
app.MapControllers();
app.MapDefaultEndpoints();

await using (var scope = app.Services.CreateAsyncScope())
{
    await scope.ServiceProvider.GetRequiredService<GameStoreDbContext>().Database.MigrateAsync();
}

await app.RunAsync();

public partial class Program;
