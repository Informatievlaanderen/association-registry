﻿namespace AssociationRegistry.Vereniging;

using Framework;

public abstract class VerenigingsBase
{
    protected VerenigingState State = new();

    public string VCode
        => State.VCode;

    public IEnumerable<IEvent> UncommittedEvents { get; private set; } = new List<IEvent>();

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
