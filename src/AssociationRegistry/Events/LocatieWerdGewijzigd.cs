namespace AssociationRegistry.Events;


using Vereniging;

public record LocatieWerdGewijzigd(
    Registratiedata.Locatie Locatie) : IEvent
{

}
