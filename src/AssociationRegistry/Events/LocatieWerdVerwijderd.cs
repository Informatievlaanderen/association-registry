namespace AssociationRegistry.Events;

public record LocatieWerdVerwijderd(string VCode,
                                    Registratiedata.Locatie Locatie) : IEvent
{

}
