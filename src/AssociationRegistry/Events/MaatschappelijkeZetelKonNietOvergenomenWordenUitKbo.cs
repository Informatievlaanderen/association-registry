namespace AssociationRegistry.Events;

using Framework;

public record MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo(
    string Straatnaam,
    string Huisnummer,
    string Busnummer,
    string Postcode,
    string Gemeente,
    string Land) : IEvent
{
}
