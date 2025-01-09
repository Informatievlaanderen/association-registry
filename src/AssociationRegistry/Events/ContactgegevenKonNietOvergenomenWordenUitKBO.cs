namespace AssociationRegistry.Events;



public record ContactgegevenKonNietOvergenomenWordenUitKBO(
    string Contactgegeventype,
    string TypeVolgensKbo,
    string Waarde) : IEvent;
