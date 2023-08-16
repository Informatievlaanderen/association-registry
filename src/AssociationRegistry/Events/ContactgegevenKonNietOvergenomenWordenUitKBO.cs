namespace AssociationRegistry.Events;

using Framework;

public record ContactgegevenKonNietOvergenomenWordenUitKBO(
    string Type,
    string Waarde) : IEvent;
