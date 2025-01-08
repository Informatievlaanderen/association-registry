namespace AssociationRegistry.Events;


using Vereniging;

public record LocatieWerdVerwijderd(string VCode,
    Registratiedata.Locatie Locatie) : IEvent
{

}
