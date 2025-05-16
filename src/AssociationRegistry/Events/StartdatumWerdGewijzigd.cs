namespace AssociationRegistry.Events;


using Vereniging;

public record StartdatumWerdGewijzigd(string VCode, DateOnly? Startdatum) : IEvent
{

}

public record StartdatumWerdGewijzigdInKbo(DateOnly? Startdatum) : IEvent
{

}
