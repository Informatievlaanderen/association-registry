namespace AssociationRegistry.Events;

public record EinddatumWerdGewijzigd(DateOnly Einddatum) : IEvent
{

}
