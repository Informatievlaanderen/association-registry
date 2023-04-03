namespace AssociationRegistry.Events;

using ContactGegevens;
using Framework;

public record ContactgegevenWerdToegevoegd(int ContactgegevenId, ContactgegevenType Type, string Waarde, string Omschrijving, bool IsPrimair) : IEvent;
