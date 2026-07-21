using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Contratos;

public sealed record ActualizarJuegoRequest(
    [StringLength(150, MinimumLength = 1)] string? Titulo,
    [StringLength(80, MinimumLength = 1)] string? Genero,
    [Range(0.01, 99999999.99)] decimal? Precio,
    [Range(0, int.MaxValue)] int? Stock);
