namespace AssociationRegistry.Events;

public record StartdatumWerdGewijzigd(string VCode, DateOnly? Startdatum) : IEvent
{

}

public record StartdatumWerdGewijzigdInKbo(DateOnly? Startdatum) : IEvent
{

}
