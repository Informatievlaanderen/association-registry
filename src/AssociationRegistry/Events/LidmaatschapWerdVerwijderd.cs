namespace AssociationRegistry.Events;

public record LidmaatschapWerdVerwijderd(string VCode,
                                         Registratiedata.Lidmaatschap Lidmaatschap) : IEvent
{

}
