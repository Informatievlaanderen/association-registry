namespace AssociationRegistry.Events;

using Framework;

public record KorteBeschrijvingWerdGewijzigd(string VCode, string KorteBeschrijving) : IEvent;

public record RoepnaamWerdGewijzigd(string Roepnaam) : IEvent;
