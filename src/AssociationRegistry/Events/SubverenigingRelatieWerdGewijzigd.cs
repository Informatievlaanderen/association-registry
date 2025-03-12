namespace AssociationRegistry.Events;

public record SubverenigingRelatieWerdGewijzigd(string VCode, string AndereVereniging, string AndereVerenigingNaam) : IEvent
{ }
