namespace AssociationRegistry.Locaties;

public record Locatie(string? Naam, string Straatnaam, string Huisnummer, string? Busnummer, string Postcode, string Gemeente, string Land, bool Hoofdlocatie, string Locatietype)
{
    public static Locatie Create(string? naam, string straatnaam, string huisnummer, string? busnummer, string postcode, string gemeente, string land, bool hoofdlocatie, string locatieType)
        => new(naam, straatnaam, huisnummer, busnummer, postcode, gemeente, land, hoofdlocatie, locatieType);
}
