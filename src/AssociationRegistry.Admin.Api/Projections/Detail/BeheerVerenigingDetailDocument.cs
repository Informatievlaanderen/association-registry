namespace AssociationRegistry.Admin.Api.Projections.Detail;

using System;
using Marten.Schema;

public record BeheerVerenigingDetailDocument : IVCode, IMetadata
{
    [Identity] public string VCode { get; init; } = null!;
    public string Naam { get; set; } = null!;
    public VerenigingsType Type { get; set; } = null!;
    public string? KorteNaam { get; set; }
    public string? KorteBeschrijving { get; set; }
    public string? Startdatum { get; set; }

    public string? Rechtsvorm { get; set; }
    public string Status { get; set; } = null!;
    public string DatumLaatsteAanpassing { get; set; } = null!;
    public Locatie[] Locaties { get; set; } = Array.Empty<Locatie>();
    public Contactgegeven[] Contactgegevens { get; set; } = Array.Empty<Contactgegeven>();
    public Vertegenwoordiger[] Vertegenwoordigers { get; set; } = Array.Empty<Vertegenwoordiger>();
    public HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; set; } = Array.Empty<HoofdactiviteitVerenigingsloket>();
    public Sleutel[] Sleutels { get; set; } = Array.Empty<Sleutel>();
    public Metadata Metadata { get; set; } = null!;
    public Relatie[] Relaties { get; set; } = Array.Empty<Relatie>();


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
        public string? Beschrijving { get; set; }
        public bool IsPrimair { get; set; }
    }

    public record Locatie
    {
        public string Locatietype { get; set; } = null!;

        public bool Hoofdlocatie { get; set; }

        public string Adres { get; set; } = null!;
        public string? Naam { get; set; }

        public string Straatnaam { get; set; } = null!;

        public string Huisnummer { get; set; } = null!;

        public string? Busnummer { get; set; }

        public string Postcode { get; set; } = null!;

        public string Gemeente { get; set; } = null!;

        public string Land { get; set; } = null!;
    }

    public record Vertegenwoordiger
    {
        public int VertegenwoordigerId { get; set; }
        public string Voornaam { get; set; } = null!;
        public string Achternaam { get; set; } = null!;
        public string? Roepnaam { get; set; }
        public string? Rol { get; set; }
        public bool IsPrimair { get; set; }
        public string Email { get; set; } = null!;
        public string Telefoon { get; set; } = null!;
        public string Mobiel { get; set; } = null!;
        public string SocialMedia { get; set; } = null!;
    }

    public record HoofdactiviteitVerenigingsloket
    {
        public string Code { get; set; } = null!;
        public string Beschrijving { get; set; } = null!;
    }

    public record Sleutel
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
