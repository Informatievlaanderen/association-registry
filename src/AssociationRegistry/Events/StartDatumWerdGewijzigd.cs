namespace AssociationRegistry.Events;

using Framework;

public record StartDatumWerdGewijzigd(string VCode, DateOnly? StartDatum) : IEvent;
