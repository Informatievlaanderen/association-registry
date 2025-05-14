namespace AssociationRegistry.Events;

public record LidmaatschapWerdToegevoegd(string VCode, Registratiedata.Lidmaatschap Lidmaatschap) : IEvent
{

}
