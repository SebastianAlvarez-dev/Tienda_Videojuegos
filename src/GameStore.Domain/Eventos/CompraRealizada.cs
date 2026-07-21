using GameStore.Domain.Common;

namespace GameStore.Domain.Eventos;

public sealed record CompraRealizada(Guid VentaId, Guid JuegoId, decimal Precio, DateTimeOffset OcurrioEn) : IEventoDominio;
