namespace AssociationRegistry.Admin.Schema.Detail;

using System;
using Constants;
using Marten.Schema;

public record Doelgroep
{
    public int Minimumleeftijd { get; init; }
    public int Maximumleeftijd { get; init; }
}

public record BeheerVerenigingDetailDocument : IVCode, IMetadata
{
    [Identity] public string VCode { get; init; } = null!;
    public string Naam { get; set; } = null!;
    public VerenigingsType Type { get; init; } = null!;
    public string? KorteNaam { get; set; }
    public string? KorteBeschrijving { get; set; }
    public string? Startdatum { get; set; }
    public Doelgroep Doelgroep { get; set; } = null!;
    public string? Rechtsvorm { get; set; }
    public string Status { get; init; } = null!;
    public string DatumLaatsteAanpassing { get; set; } = null!;
    public Locatie[] Locaties { get; set; } = Array.Empty<Locatie>();
    public Contactgegeven[] Contactgegevens { get; set; } = Array.Empty<Contactgegeven>();
    public Vertegenwoordiger[] Vertegenwoordigers { get; set; } = Array.Empty<Vertegenwoordiger>();
    public HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; set; } = Array.Empty<HoofdactiviteitVerenigingsloket>();
    public Sleutel[] Sleutels { get; init; } = Array.Empty<Sleutel>();
    public Metadata Metadata { get; set; } = null!;
    public Relatie[] Relaties { get; init; } = Array.Empty<Relatie>();
    public bool IsUitgeschrevenUitPubliekeDatastroom { get; set; }
    public Bron Bron { get; set; } = null!;


    public record VerenigingsType
    {
        public string Code { get; init; } = null!;
        public string Beschrijving { get; init; } = null!;
    }

    public record Contactgegeven
    {
        public int ContactgegevenId { get; init; }
        public string Type { get; init; } = null!;
        public string Waarde { get; init; } = null!;
        public string? Beschrijving { get; init; }
        public bool IsPrimair { get; init; }
        public Bron Bron { get; set; } = null!;
    }

    public record Locatie
    {
        public int LocatieId { get; set; }
        public string Locatietype { get; set; } = null!;
        public bool IsPrimair { get; set; }
        public string Adresvoorstelling { get; init; } = null!;
        public Adres? Adres { get; set; }
        public string? Naam { get; set; }
        public AdresId? AdresId { get; set; }
        public Bron Bron { get; set; } = null!;
    }

    public record Vertegenwoordiger
    {
        public int VertegenwoordigerId { get; init; }
        public string Voornaam { get; init; } = null!;
        public string Achternaam { get; init; } = null!;
        public string? Roepnaam { get; init; }
        public string? Rol { get; init; }
        public bool IsPrimair { get; init; }
        public string Email { get; init; } = null!;
        public string Telefoon { get; init; } = null!;
        public string Mobiel { get; init; } = null!;
        public string SocialMedia { get; init; } = null!;
        public Bron Bron { get; set; } = null!;
    }

    public record HoofdactiviteitVerenigingsloket
    {
        public string Code { get; init; } = null!;
        public string Beschrijving { get; init; } = null!;
    }

    public record Sleutel
    {
        public string Bron { get; init; } = null!;
        public string Waarde { get; init; } = null!;
    }

    public class Relatie
    {
        public string Type { get; init; } = null!;

        public GerelateerdeVereniging AndereVereniging { get; init; } = null!;

        public class GerelateerdeVereniging
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
