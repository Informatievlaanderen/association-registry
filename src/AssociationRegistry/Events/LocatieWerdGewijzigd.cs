namespace AssociationRegistry.Events;

public record LocatieWerdGewijzigd(
    Registratiedata.Locatie Locatie) : IEvent
{

}
