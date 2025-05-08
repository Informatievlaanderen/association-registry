namespace AssociationRegistry.Events;

public record MarkeringDubbeleVerengingWerdGecorrigeerd(string VCode, string VCodeAuthentiekeVereniging, string VorigeStatus) : IEvent
{
}


