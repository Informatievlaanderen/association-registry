namespace AssociationRegistry.Events;

using Framework;

public record ContactgegevenWerdGewijzigd(int ContactgegevenId, string Type, string Waarde, string Omschrijving, bool IsPrimair) : IEvent;
