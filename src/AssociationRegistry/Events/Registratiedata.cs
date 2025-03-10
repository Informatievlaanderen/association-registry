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

    }

    public record Locatie(
        int LocatieId,
        string Locatietype,
        bool IsPrimair,
        string Naam,
        Adres? Adres,
        AdresId? AdresId)
    {
    }

    public record Adres(
        string Straatnaam,
        string Huisnummer,
        string Busnummer,
        string Postcode,
        string Gemeente,
        string Land)
    {

    }

    public record AdresId(string Broncode, string Bronwaarde)
    {
        public bool Equals(IAdresId adresId)
            => this == adresId;

        public static bool operator ==(AdresId first, IAdresId second)
            => first.Broncode == second.Adresbron.Code && first.Bronwaarde == second.Bronwaarde;

        public static bool operator !=(AdresId first, IAdresId second)
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

    }

    public record Doelgroep(int Minimumleeftijd, int Maximumleeftijd)
    {

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

    }

    public record HoofdactiviteitVerenigingsloket(
        string Code,
        string Naam)
    {

    }

    public record Werkingsgebied(
        string Code,
        string Naam)
    {

    }

    public record Lidmaatschap(
        int LidmaatschapId,
        string AndereVereniging,
        string AndereVerenigingNaam,
        DateOnly? DatumVan,
        DateOnly? DatumTot,
        string Identificatie,
        string Beschrijving)
    {

    }
}

public interface IAdresId
{
    IAdresbron Adresbron { get; }

    string Bronwaarde { get; }
}

public interface IAdresbron
{
    string Code { get; }
    string Beschrijving { get; }
}
