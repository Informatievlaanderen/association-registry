namespace AssociationRegistry.Events;

using Framework;

public record ContactgegevenKonNietOvergenomenWordenUitKBO(
    string Contactgegeventype,
    string TypeVolgensKbo,
    string Waarde) : IEvent;
