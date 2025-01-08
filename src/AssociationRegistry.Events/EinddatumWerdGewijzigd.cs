namespace AssociationRegistry.Events;


using Vereniging;

public record EinddatumWerdGewijzigd(DateOnly Einddatum) : IEvent
{

}
