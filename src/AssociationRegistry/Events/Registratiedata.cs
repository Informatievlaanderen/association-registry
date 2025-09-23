namespace AssociationRegistry.Events;

using Marten.Linq.SqlGeneration.Filters;

public static class Registratiedata
{
    public record Contactgegeven(
        int ContactgegevenId,
        string Contactgegeventype,
        string Waarde,
        string Beschrijving,
        bool IsPrimair);

    public record Locatie(
        int LocatieId,
        string Locatietype,
        bool IsPrimair,
        string Naam,
        Adres? Adres,
        AdresId? AdresId);

    public record Adres(
        string Straatnaam,
        string Huisnummer,
        string Busnummer,
        string Postcode,
        string Gemeente,
        string Land);

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

        public string ToId()
            => new Uri(Bronwaarde).Segments.Last();

        public static AdresId FromAdresId(DecentraalBeheer.Vereniging.Adressen.AdresId adres)
            => new(adres.Adresbron.Code, adres.Bronwaarde);
    }

    public record AdresUitAdressenregister(
        string Straatnaam,
        string Huisnummer,
        string Busnummer,
        string Postcode,
        string Gemeente)
    {
        public static AdresUitAdressenregister FromAdres(DecentraalBeheer.Vereniging.Adressen.Adres adres)
            => new(adres.Straatnaam, adres.Huisnummer, adres.Busnummer, adres.Postcode, adres.Gemeente.Naam);
    };

    public record Doelgroep(int Minimumleeftijd, int Maximumleeftijd);

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
        string SocialMedia);

    public record HoofdactiviteitVerenigingsloket(
        string Code,
        string Naam);

    public record Werkingsgebied(
        string Code,
        string Naam);

    public record Lidmaatschap(
        int LidmaatschapId,
        string AndereVereniging,
        string AndereVerenigingNaam,
        DateOnly? DatumVan,
        DateOnly? DatumTot,
        string Identificatie,
        string Beschrijving);

    public record SubverenigingVan(
        string AndereVereniging,
        string AndereVerenigingNaam,
        string Identificatie,
        string Beschrijving);

    public record DuplicatieInfo(bool? BevestigdNaDuplicatie, string Bevestigingstoken, string BevestigingstokenKey)
    {
        public static DuplicatieInfo Onbekend = new(null, string.Empty, string.Empty);
        public static DuplicatieInfo GeenDuplicaten = new(false, string.Empty, string.Empty);
        public static DuplicatieInfo BevestigdGeenDuplicaat(string bevestigingstoken, string bevestigingstokenKey)
            => new(true, bevestigingstoken, bevestigingstokenKey);

        public static implicit operator bool?(DuplicatieInfo info) => info?.BevestigdNaDuplicatie;
    }

    public record Geotag(string Identificiatie);

    public record DuplicateVereniging(
        string VCode,
        Verenigingstype Verenigingstype,
        Verenigingssubtype? Verenigingssubtype,
        string Naam,
        string KorteNaam,
        HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket,
        DuplicateVerenigingLocatie[] Locaties);

    public record Verenigingstype(string Code, string Naam);

    public record Verenigingssubtype(string Code, string Naam);

    public record DuplicateVerenigingLocatie(
        string Locatietype,
        bool IsPrimair,
        string Adres,
        string? Naam,
        string Postcode,
        string Gemeente);
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
