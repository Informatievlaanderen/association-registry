namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt(string VCode) : IEvent
{
    public static WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt With(VCode vCode)
        => new(vCode);
}


