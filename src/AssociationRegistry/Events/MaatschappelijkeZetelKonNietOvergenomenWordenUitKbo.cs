namespace AssociationRegistry.Events;

using Framework;
using Kbo;

public record MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo(
    string Straatnaam,
    string Huisnummer,
    string Busnummer,
    string Postcode,
    string Gemeente,
    string Land) : IEvent
{
    public static MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo With(AdresVolgensKbo adres)
        => new(
            adres.Straatnaam ?? string.Empty,
            adres.Huisnummer ?? string.Empty,
            adres.Busnummer ?? string.Empty,
            adres.Postcode ?? string.Empty,
            adres.Gemeente ?? string.Empty,
            adres.Land ?? string.Empty);
}
