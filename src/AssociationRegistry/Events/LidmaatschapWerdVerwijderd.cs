namespace AssociationRegistry.Events;


using Vereniging;

public record LidmaatschapWerdVerwijderd(string VCode,
                                         Registratiedata.Lidmaatschap Lidmaatschap) : IEvent
{

}
