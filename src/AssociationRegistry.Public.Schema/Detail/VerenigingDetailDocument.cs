namespace AssociationRegistry.Public.Schema.Detail;

using System;
using Marten.Schema;

public record PubliekVerenigingDetailDocument : IVCode
{
    [Identity] public string VCode { get; set; } = null!;

    public VerenigingsType Type { get; set; } = null!;
    public string Naam { get; set; } = null!;
    public string KorteNaam { get; set; } = null!;
    public string KorteBeschrijving { get; set; } = null!;
    public DateOnly? Startdatum { get; set; }
    public string Status { get; set; } = null!;
    public string DatumLaatsteAanpassing { get; set; } = null!;
    public Locatie[] Locaties { get; set; } = null!;
    public Contactgegeven[] Contactgegevens { get; set; } = Array.Empty<Contactgegeven>();
    public HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; set; } = Array.Empty<HoofdactiviteitVerenigingsloket>();

    public record VerenigingsType
    {
        public string Code { get; set; } = null!;
        public string Beschrijving { get; set; } = null!;
    }

    public record Contactgegeven
    {
        public int ContactgegevenId { get; set; }
        public string Type { get; set; } = null!;
        public string Waarde { get; set; } = null!;
        public string Beschrijving { get; set; } = null!;
        public bool IsPrimair { get; set; }
    }

    public record Locatie
    {
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
    }

    public record HoofdactiviteitVerenigingsloket
    {
        public string Code { get; set; } = null!;
        public string Beschrijving { get; set; } = null!;
    }
}
