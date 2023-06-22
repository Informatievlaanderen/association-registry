namespace AssociationRegistry.Events;

public static class Registratiedata
{
    public record Contactgegeven(
        int ContactgegevenId,
        string Type,
        string Waarde,
        string Beschrijving,
        bool IsPrimair)
    {
        public static Contactgegeven With(AssociationRegistry.Vereniging.Contactgegeven contactgegeven)
            => new(
                contactgegeven.ContactgegevenId,
                contactgegeven.Type,
                contactgegeven.Waarde,
                contactgegeven.Beschrijving,
                contactgegeven.IsPrimair);
    }

    public record Locatie(
        int LocatieId,
        string Naam,
        Adres Adres,
        bool Hoofdlocatie,
        string Locatietype,
        AdresId? AdresId = null)
    {
        public static Locatie With(AssociationRegistry.Vereniging.Locatie locatie)
            => new(
                locatie.LocatieId,
                locatie.Naam ?? string.Empty,
                new Adres(
                    locatie.Adres!.Straatnaam,
                    locatie.Adres.Huisnummer,
                    locatie.Adres.Busnummer ?? string.Empty,
                    locatie.Adres.Postcode,
                    locatie.Adres.Gemeente,
                    locatie.Adres.Land),
                locatie.Hoofdlocatie,
                locatie.Locatietype,
                locatie.AdresId is not null ? new AdresId(locatie.AdresId.Adresbron, locatie.AdresId.Bronwaarde) : null);
    }

    public record Adres(
        string Straatnaam,
        string Huisnummer,
        string Busnummer,
        string Postcode,
        string Gemeente,
        string Land);

    public record AdresId(string Broncode, string Bronwaarde);

    public record Vertegenwoordiger(
        int VertegenwoordigerId,
        string Insz,
        bool IsPrimair,
        string Roepnaam,
        string Rol,
        string Voornaam,
        string Achternaam,
        string Email,
        string Telefoon,
        string Mobiel,
        string SocialMedia)
    {
        public static Vertegenwoordiger With(AssociationRegistry.Vereniging.Vertegenwoordiger vertegenwoordiger)
            => new(
                vertegenwoordiger.VertegenwoordigerId,
                vertegenwoordiger.Insz,
                vertegenwoordiger.IsPrimair,
                vertegenwoordiger.Roepnaam ?? string.Empty,
                vertegenwoordiger.Rol ?? string.Empty,
                vertegenwoordiger.Voornaam,
                vertegenwoordiger.Achternaam,
                vertegenwoordiger.Email.Waarde,
                vertegenwoordiger.Telefoon.Waarde,
                vertegenwoordiger.Mobiel.Waarde,
                vertegenwoordiger.SocialMedia.Waarde);
    }

    public record HoofdactiviteitVerenigingsloket(
        string Code,
        string Beschrijving)
    {
        public static HoofdactiviteitVerenigingsloket With(AssociationRegistry.Vereniging.HoofdactiviteitVerenigingsloket activiteit)
            => new(activiteit.Code, activiteit.Beschrijving);
    }
}
