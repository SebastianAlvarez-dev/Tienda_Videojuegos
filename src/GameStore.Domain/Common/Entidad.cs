namespace GameStore.Domain.Common;

public abstract class Entidad
{
    private readonly List<IEventoDominio> _eventosDominio = [];

    public IReadOnlyCollection<IEventoDominio> EventosDominio => _eventosDominio.AsReadOnly();

    protected void RegistrarEvento(IEventoDominio eventoDominio) => _eventosDominio.Add(eventoDominio);

    public IReadOnlyCollection<IEventoDominio> ExtraerEventosDominio()
    {
        var eventos = _eventosDominio.ToArray();
        _eventosDominio.Clear();
        return eventos;
    }
}
