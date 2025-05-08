namespace AssociationRegistry.Events;

public record LidmaatschapWerdGewijzigd(string VCode, Registratiedata.Lidmaatschap Lidmaatschap) : IEvent
{

}
