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
                contactgegeven.Waarde.Waarde,
                contactgegeven.Beschrijving,
                contactgegeven.IsPrimair);
    }

    public record Locatie(
        int LocatieId,
        string Locatietype,
        bool IsPrimair,
        string Naam,
        Adres? Adres,
        AdresId? AdresId)
    {
        public static Locatie With(AssociationRegistry.Vereniging.Locatie locatie)
            => new(
                locatie.LocatieId,
                locatie.Locatietype,
                locatie.IsPrimair,
                locatie.Naam ?? string.Empty,
                Adres.With(locatie.Adres),
                locatie.AdresId is not null ? new AdresId(locatie.AdresId.Adresbron, locatie.AdresId.Bronwaarde) : null);
    }

    public record Adres(
        string Straatnaam,
        string Huisnummer,
        string Busnummer,
        string Postcode,
        string Gemeente,
        string Land)
    {
        public static Adres? With(AssociationRegistry.Vereniging.Adres? adres)
        {
            if (adres is null)
                return null;

            return new Adres(
                adres.Straatnaam,
                adres.Huisnummer,
                adres.Busnummer,
                adres.Postcode,
                adres.Gemeente,
                adres.Land);
        }
    }

    public record AdresId(string Broncode, string Bronwaarde)
    {
        public static AdresId? With(AssociationRegistry.Vereniging.AdresId? adresId)
        {
            if (adresId is null)
                return null;

            return new AdresId(
                adresId.Adresbron.Code,
                adresId.Bronwaarde);
        }
    }

    public record Doelgroep(int Minimumleeftijd, int Maximumleeftijd)
    {
        public static Doelgroep With(AssociationRegistry.Vereniging.Doelgroep doelgroep)
            => new(doelgroep.Minimumleeftijd, doelgroep.Maximumleeftijd);
    }

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
