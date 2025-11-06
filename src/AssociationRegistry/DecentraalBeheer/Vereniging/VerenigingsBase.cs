namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using Events;
using Persoonsgegevens;

public abstract class VerenigingsBase
{
    public VerenigingsBase()
    {
        State = new VerenigingState();
    }

    public VerenigingState State { get; set; }

    public VCode VCode
        => State.VCode;
    public string Naam => State.Naam;

    public IEnumerable<IEvent> UncommittedEvents { get; private set; } = new List<IEvent>();
    public long Version => State.Version;

    public void ClearEvents()
    {
        UncommittedEvents = new List<IEvent>();
    }

    protected void AddEvent(IEvent @event)
    {
        Apply(@event);
        UncommittedEvents = UncommittedEvents.Append(@event);
    }

    public void Apply(dynamic @event)
    {
        State = State.Apply(@event);
    }
}
