namespace AssociationRegistry.Events;


using Vereniging;

public record WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt(string VCode, string VCodeAuthentiekeVereniging, string VorigeStatus) : IEvent
{
}


