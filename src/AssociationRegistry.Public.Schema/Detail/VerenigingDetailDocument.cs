namespace AssociationRegistry.Public.Schema.Detail;

using System;
using Marten.Schema;

public class PubliekVerenigingDetailDocument : IVCode, ICanBeUitgeschrevenUitPubliekeDatastroom
{
    [Identity] public string VCode { get; set; } = null!;

    public VerenigingsType Type { get; set; } = null!;
    public string Naam { get; set; } = null!;
    public string KorteNaam { get; set; } = null!;
    public string KorteBeschrijving { get; set; } = null!;
    public DateOnly? Startdatum { get; set; }

    public string? Rechtsvorm { get; set; }
    public string Status { get; set; } = null!;
    public string DatumLaatsteAanpassing { get; set; } = null!;
    public Locatie[] Locaties { get; set; } = null!;
    public Contactgegeven[] Contactgegevens { get; set; } = Array.Empty<Contactgegeven>();
    public HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; set; } = Array.Empty<HoofdactiviteitVerenigingsloket>();
    public Sleutel[] Sleutels { get; set; } = Array.Empty<Sleutel>();
    public Relatie[] Relaties { get; set; } = Array.Empty<Relatie>();
    public bool IsUitgeschrevenUitPubliekeDatastroom { get; set; }

    public class VerenigingsType
    {
        public string Code { get; set; } = null!;
        public string Beschrijving { get; set; } = null!;
    }

    public class Contactgegeven
    {
        public int ContactgegevenId { get; set; }
        public string Type { get; set; } = null!;
        public string Waarde { get; set; } = null!;
        public string Beschrijving { get; set; } = null!;
        public bool IsPrimair { get; set; }
    }

    public record Locatie
    {
        public string Locatietype { get; init; } = null!;
        public bool IsPrimair { get; init; }
        public string Adresvoorstelling { get; init; } = null!;
        public Adres? Adres { get; init; } = null!;
        public string? Naam { get; init; }
        public AdresId? AdresId { get; set; }
    }

    public class HoofdactiviteitVerenigingsloket
    {
        public string Code { get; set; } = null!;
        public string Beschrijving { get; set; } = null!;
    }

    public class Sleutel
    {
        public string Bron { get; set; } = null!;
        public string Waarde { get; set; } = null!;
    }

    public class Relatie
    {
        public string Type { get; set; } = null!;

        public GerelateerdeVereniging AndereVereniging { get; set; } = null!;

        public class GerelateerdeVereniging
        {
            public string KboNummer { get; set; } = null!;

            public string VCode { get; set; } = null!;

            public string Naam { get; set; } = null!;
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
}
