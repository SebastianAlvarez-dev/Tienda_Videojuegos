using GameStore.Api.Contratos;
using GameStore.Application.Features.Compras.RegistrarCompra;
using GameStore.Application.Features.Juegos.ActualizarJuego;
using GameStore.Application.Features.Juegos.CrearJuego;
using GameStore.Application.Features.Juegos.ListarJuegos;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Api.Controllers;

[ApiController]
[Route("juegos")]
public sealed class JuegosController(
    CrearJuegoHandler crearJuego,
    ListarJuegosHandler listarJuegos,
    ActualizarJuegoHandler actualizarJuego,
    RegistrarCompraHandler registrarCompra) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Crear(
        CrearJuegoRequest request,
        CancellationToken cancellationToken)
    {
        var juego = await crearJuego.ManejarAsync(
            new CrearJuegoCommand(request.Titulo, request.Genero, request.Precio, request.Stock),
            cancellationToken);
        return StatusCode(StatusCodes.Status201Created, juego);
    }

    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken) =>
        Ok(await listarJuegos.ManejarAsync(new ListarJuegosQuery(), cancellationToken));

    [HttpPatch("{juegoId:guid}")]
    public async Task<IActionResult> Actualizar(
        Guid juegoId,
        ActualizarJuegoRequest request,
        CancellationToken cancellationToken) =>
        Ok(await actualizarJuego.ManejarAsync(
            new ActualizarJuegoCommand(juegoId, request.Titulo, request.Genero, request.Precio, request.Stock),
            cancellationToken));

    [HttpPost("{juegoId:guid}/compras")]
    public async Task<IActionResult> Comprar(Guid juegoId, CancellationToken cancellationToken)
    {
        var venta = await registrarCompra.ManejarAsync(
            new RegistrarCompraCommand(juegoId),
            cancellationToken);
        return StatusCode(StatusCodes.Status201Created, venta);
    }
}
