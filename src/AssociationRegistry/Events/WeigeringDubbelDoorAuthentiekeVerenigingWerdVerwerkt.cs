namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt(string VCode, string VCodeAuthentiekeVereniging, string VorigeStatus) : IEvent
{
    public static WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt With(VCode vCode, VCode vCodeAuthentiekeVereniging, VerenigingStatus vorigeStatus)
        => new(vCode, vCodeAuthentiekeVereniging,vorigeStatus.Naam);
}


