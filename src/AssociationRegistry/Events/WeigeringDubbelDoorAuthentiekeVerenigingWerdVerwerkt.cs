namespace AssociationRegistry.Events;

public record WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt(string VCode, string VCodeAuthentiekeVereniging, string VorigeStatus) : IEvent
{
}


