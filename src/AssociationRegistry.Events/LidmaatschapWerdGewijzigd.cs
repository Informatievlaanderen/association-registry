namespace AssociationRegistry.Events;


using Vereniging;

public record LidmaatschapWerdGewijzigd(string VCode, Registratiedata.Lidmaatschap Lidmaatschap) : IEvent
{

}
