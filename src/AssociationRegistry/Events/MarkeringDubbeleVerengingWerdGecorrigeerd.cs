namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record MarkeringDubbeleVerengingWerdGecorrigeerd(string VCode, string VCodeAuthentiekeVereniging, string VorigeStatus) : IEvent
{
    public static MarkeringDubbeleVerengingWerdGecorrigeerd With(VCode vCode, VCode vCodeAuthentiekeVereniging, VerenigingStatus vorigeStatus)
        => new(vCode, vCodeAuthentiekeVereniging,vorigeStatus.Naam);
}


