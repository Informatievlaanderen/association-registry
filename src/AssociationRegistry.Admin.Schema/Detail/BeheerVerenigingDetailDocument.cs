namespace AssociationRegistry.Admin.Schema.Detail;

using Marten.Schema;

public record Doelgroep
{
    public int Minimumleeftijd { get; init; }
    public int Maximumleeftijd { get; init; }
}

public record BeheerVerenigingDetailDocument : IVCode, IMetadata
{
    public string Naam { get; set; } = null!;
    public VerenigingsType Verenigingstype { get; init; } = null!;
    public string Roepnaam { get; set; } = null!;
    public string? KorteNaam { get; set; }
    public string? KorteBeschrijving { get; set; }
    public string? Startdatum { get; set; }
    public string? Einddatum { get; set; }
    public Doelgroep Doelgroep { get; set; } = null!;
    public string? Rechtsvorm { get; set; }
    public string Status { get; set; } = null!;
    public string DatumLaatsteAanpassing { get; set; } = null!;
    public Locatie[] Locaties { get; set; } = Array.Empty<Locatie>();
    public Contactgegeven[] Contactgegevens { get; set; } = Array.Empty<Contactgegeven>();
    public Vertegenwoordiger[] Vertegenwoordigers { get; set; } = Array.Empty<Vertegenwoordiger>();

    public HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; set; } =
        Array.Empty<HoofdactiviteitVerenigingsloket>();

    public Sleutel[] Sleutels { get; set; } = Array.Empty<Sleutel>();
    public Relatie[] Relaties { get; set; } = Array.Empty<Relatie>();
    public bool IsUitgeschrevenUitPubliekeDatastroom { get; set; }
    public string Bron { get; set; } = null!;
    public Metadata Metadata { get; set; } = null!;
    [Identity] public string VCode { get; init; } = null!;

    public record VerenigingsType
    {
        public string Code { get; init; } = null!;
        public string Naam { get; init; } = null!;
    }

    public record Contactgegeven : IHasBron
    {
        public int ContactgegevenId { get; init; }
        public string Contactgegeventype { get; init; } = null!;
        public string Waarde { get; init; } = null!;
        public string? Beschrijving { get; init; }
        public bool IsPrimair { get; init; }
        public string Bron { get; set; } = null!;
    }

    public record Locatie : IHasBron
    {
        public int LocatieId { get; set; }
        public string Locatietype { get; set; } = null!;
        public bool IsPrimair { get; set; }
        public string Adresvoorstelling { get; init; } = null!;
        public Adres? Adres { get; set; }
        public string? Naam { get; set; }
        public AdresId? AdresId { get; set; }
        public string Bron { get; set; } = null!;
    }

    public record Vertegenwoordiger : IHasBron
    {
        public int VertegenwoordigerId { get; init; }
        public string Insz { get; set; } = null!;
        public string Voornaam { get; init; } = null!;
        public string Achternaam { get; init; } = null!;
        public string? Roepnaam { get; init; }
        public string? Rol { get; init; }
        public bool IsPrimair { get; init; }
        public string Email { get; init; } = null!;
        public string Telefoon { get; init; } = null!;
        public string Mobiel { get; init; } = null!;
        public string SocialMedia { get; init; } = null!;

        public int Identity
            => VertegenwoordigerId;

        public string Bron { get; set; } = null!;
    }

    public record HoofdactiviteitVerenigingsloket
    {
        public string Code { get; init; } = null!;
        public string Naam { get; init; } = null!;
    }

    public record Sleutel
    {
        public string Bron { get; init; } = null!;
        public string Waarde { get; init; } = null!;
    }

    public record Relatie
    {
        public string Relatietype { get; init; } = null!;
        public GerelateerdeVereniging AndereVereniging { get; init; } = null!;

        public record GerelateerdeVereniging
        {
            public string KboNummer { get; init; } = null!;
            public string VCode { get; init; } = null!;
            public string Naam { get; init; } = null!;
        }
    }
}

public class AdresId
{
    public string? Broncode { get; set; }
    public string? Bronwaarde { get; set; }
}

public class Adres
{
    public string Straatnaam { get; init; } = null!;
    public string Huisnummer { get; init; } = null!;
    public string? Busnummer { get; init; }
    public string Postcode { get; init; } = null!;
    public string Gemeente { get; init; } = null!;
    public string Land { get; init; } = null!;
}

public interface IHasBron
{
    public string Bron { get; set; }
}
