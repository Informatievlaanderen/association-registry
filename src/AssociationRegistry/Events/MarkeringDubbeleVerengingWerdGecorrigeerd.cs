namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record MarkeringDubbeleVerengingWerdGecorrigeerd(string VCode, string VCodeAuthentiekeVereniging, string VorigeStatus) : IEvent
{
    public static MarkeringDubbeleVerengingWerdGecorrigeerd With(VCode vCode, VerenigingStatus.StatusDubbel verenigingStatus)
        => new(vCode, verenigingStatus.VCodeAuthentiekeVereniging, verenigingStatus.StatusNaam);
}


