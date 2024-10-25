namespace AssociationRegistry.Events;

public static class Registratiedata
{
    public record Contactgegeven(
        int ContactgegevenId,
        string Contactgegeventype,
        string Waarde,
        string Beschrijving,
        bool IsPrimair)
    {
        public static Contactgegeven With(Vereniging.Contactgegeven contactgegeven)
            => new(
                contactgegeven.ContactgegevenId,
                contactgegeven.Contactgegeventype,
                contactgegeven.Waarde,
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
        public static Locatie With(Vereniging.Locatie locatie)
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
        public static Adres? With(Vereniging.Adres? adres)
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
        public static AdresId? With(Vereniging.AdresId? adresId)
        {
            if (adresId is null)
                return null;

            return new AdresId(
                adresId.Adresbron.Code,
                adresId.Bronwaarde);
        }

        public bool Equals(Vereniging.AdresId adresId)
            => this == adresId;

        public static bool operator ==(AdresId first, Vereniging.AdresId second)
            => first.Broncode == second.Adresbron.Code && first.Bronwaarde == second.Bronwaarde;

        public static bool operator !=(AdresId first, Vereniging.AdresId second)
            => first.Broncode != second.Adresbron.Code || first.Bronwaarde != second.Bronwaarde;

        public override string ToString()
            => new Uri(Bronwaarde).Segments.Last();
    }

    public record AdresUitAdressenregister(
        string Straatnaam,
        string Huisnummer,
        string Busnummer,
        string Postcode,
        string Gemeente)
    {
        public static AdresUitAdressenregister? With(AdresDetailUitAdressenregister? adres)
        {
            if (adres is null)
                return null;

            return new AdresUitAdressenregister(
                adres.Adres.Straatnaam,
                adres.Adres.Huisnummer,
                adres.Adres.Busnummer,
                adres.Adres.Postcode,
                adres.Adres.Gemeente);
        }
    }

    public record Doelgroep(int Minimumleeftijd, int Maximumleeftijd)
    {
        public static Doelgroep With(Vereniging.Doelgroep doelgroep)
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
        public static Vertegenwoordiger With(Vereniging.Vertegenwoordiger vertegenwoordiger)
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
        string Naam)
    {
        public static HoofdactiviteitVerenigingsloket With(Vereniging.HoofdactiviteitVerenigingsloket activiteit)
            => new(activiteit.Code, activiteit.Naam);
    }

    public record Werkingsgebied(
        string Code,
        string Naam)
    {
        public static Werkingsgebied With(Vereniging.Werkingsgebied werkingsgebied)
            => new(werkingsgebied.Code, werkingsgebied.Naam);
    }
}
