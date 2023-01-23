namespace AssociationRegistry.Events;

using Framework;

public record KorteBeschrijvingWerdGewijzigd(string VCode, string KorteBeschrijving) : IEvent;
