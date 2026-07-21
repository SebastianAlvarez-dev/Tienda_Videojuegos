using GameStore.Application.Abstracciones;
using GameStore.Application.Contratos;

namespace GameStore.Application.Features.Compras.RegistrarCompra;

public sealed record RegistrarCompraCommand(Guid JuegoId) : IComando<VentaRespuesta>;
