namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt(string VCode, string VCodeAuthentiekeVereniging, string VorigeStatus) : IEvent
{
    public static WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt With(VCode vCode, VerenigingStatus.StatusDubbel verenigingStatus)
        => new(vCode, verenigingStatus.VCodeAuthentiekeVereniging, verenigingStatus.StatusNaam);
}


