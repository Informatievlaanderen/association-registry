namespace AssociationRegistry.Events;

using Framework;

public record StartdatumWerdGewijzigd(string VCode, DateOnly? Startdatum) : IEvent;
