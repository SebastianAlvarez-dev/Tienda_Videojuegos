using GameStore.Domain.Common;

namespace GameStore.Domain.Eventos;

public sealed record JuegoCreado(Guid JuegoId, string Titulo, DateTimeOffset OcurrioEn) : IEventoDominio;
