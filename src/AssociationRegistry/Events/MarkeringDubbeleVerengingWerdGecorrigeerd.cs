namespace AssociationRegistry.Events;


using Vereniging;

public record MarkeringDubbeleVerengingWerdGecorrigeerd(string VCode, string VCodeAuthentiekeVereniging, string VorigeStatus) : IEvent
{
}


