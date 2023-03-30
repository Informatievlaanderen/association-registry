namespace AssociationRegistry.Events;

using Framework;

public record ContactgegevenWerdToegevoegd(int ContactgegevenId, string Type, string Waarde, string Omschrijving, bool IsPrimair) : IEvent;
