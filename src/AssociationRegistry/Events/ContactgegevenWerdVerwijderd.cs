namespace AssociationRegistry.Events;

using ContactGegevens;
using Framework;

public record ContactgegevenWerdVerwijderd(int ContactgegevenId, ContactgegevenType Type, string Waarde, string Omschrijving, bool IsPrimair) : IEvent;
