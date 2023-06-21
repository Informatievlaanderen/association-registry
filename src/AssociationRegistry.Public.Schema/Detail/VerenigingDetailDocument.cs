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

    public class Locatie
    {
        public int LocatieId { get; set; }
        public string Locatietype { get; set; } = null!;
        public bool Hoofdlocatie { get; set; }
        public string Adres { get; set; } = null!;
        public string Naam { get; set; } = null!;
        public string Straatnaam { get; set; } = null!;
        public string Huisnummer { get; set; } = null!;
        public string Busnummer { get; set; } = null!;
        public string Postcode { get; set; } = null!;
        public string Gemeente { get; set; } = null!;
        public string Land { get; set; } = null!;
        public string? AdresId { get; set; }
        public string? Adresbron { get; set; }
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
}
