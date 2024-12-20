namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt(string VCode, string VorigeStatus) : IEvent
{
    public static WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt With(VCode vCode, VerenigingStatus vorigeStatus)
        => new(vCode, vorigeStatus.Naam);
}


