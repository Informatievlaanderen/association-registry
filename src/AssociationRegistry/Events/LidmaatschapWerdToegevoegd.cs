namespace AssociationRegistry.Events;


using Vereniging;

public record LidmaatschapWerdToegevoegd(string VCode, Registratiedata.Lidmaatschap Lidmaatschap) : IEvent
{

}
