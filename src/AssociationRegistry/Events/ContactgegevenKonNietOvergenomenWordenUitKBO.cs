namespace AssociationRegistry.Events;

using Framework;

public record ContactgegevenKonNietOvergenomenWordenUitKBO(
    string Type,
    string TypeVolgensKbo,
    string Waarde) : IEvent;
