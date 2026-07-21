using GameStore.Domain.Common;

namespace GameStore.Domain.Eventos;

public sealed record JuegoActualizado(Guid JuegoId, DateTimeOffset OcurrioEn) : IEventoDominio;
