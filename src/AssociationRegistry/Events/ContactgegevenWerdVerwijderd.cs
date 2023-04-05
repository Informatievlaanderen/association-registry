namespace AssociationRegistry.Events;

using Framework;

public record ContactgegevenWerdVerwijderd(int ContactgegevenId, string Type, string Waarde, string Omschrijving, bool IsPrimair) : IEvent;
