namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using Events;

public abstract class VerenigingsBase
{
    protected VerenigingState State = new();

    /// <summary>Alleen voor het domein: de authentieke vereniging leest de gegevens van de dubbele om ze samen te voegen.</summary>
    internal VerenigingState CurrentState => State;

    public VCode VCode => State.VCode;
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
