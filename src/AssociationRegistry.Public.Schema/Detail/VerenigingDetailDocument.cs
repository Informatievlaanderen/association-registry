﻿namespace AssociationRegistry.Public.Schema.Detail;

using Marten.Metadata;
using Marten.Schema;

public record Doelgroep
{
    public int Minimumleeftijd { get; set; }
    public int Maximumleeftijd { get; set; }
}

public class PubliekVerenigingDetailDocument : IVCode, ISoftDeleted, ICanBeUitgeschrevenUitPubliekeDatastroom
{
    public JsonLdMetadata JsonLdMetadata { get; set; } = null!;

    public VerenigingsType Verenigingstype { get; set; } = null!;
    public string Naam { get; set; } = null!;
    public string? Roepnaam { get; set; }
    public string KorteNaam { get; set; } = null!;
    public string KorteBeschrijving { get; set; } = null!;
    public DateOnly? Startdatum { get; set; }
    public Doelgroep Doelgroep { get; set; } = null!;
    public string? Rechtsvorm { get; set; }
    public string Status { get; set; } = null!;
    public string DatumLaatsteAanpassing { get; set; } = null!;
    public Locatie[] Locaties { get; set; } = null!;
    public Contactgegeven[] Contactgegevens { get; set; } = Array.Empty<Contactgegeven>();

    public HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; set; } =
        Array.Empty<HoofdactiviteitVerenigingsloket>();

    public Sleutel[] Sleutels { get; set; } = Array.Empty<Sleutel>();
    public Relatie[] Relaties { get; set; } = Array.Empty<Relatie>();
    public bool IsUitgeschrevenUitPubliekeDatastroom { get; set; }
    [Identity] public string VCode { get; set; } = null!;

    public class VerenigingsType
    {
        public string Code { get; set; } = null!;
        public string Naam { get; set; } = null!;
    }

    public class Contactgegeven
    {
        public JsonLdMetadata JsonLdMetadata { get; set; }
        public int ContactgegevenId { get; set; }
        public string Contactgegeventype { get; set; } = null!;
        public string Waarde { get; set; } = null!;
        public string Beschrijving { get; set; } = null!;
        public bool IsPrimair { get; set; }
    }

    public record Locatie
    {
        public JsonLdMetadata JsonLdMetadata { get; set; } = null!;
        public int LocatieId { get; set; }
        public LocatieType Locatietype { get; set; } = null!;
        public bool IsPrimair { get; set; }
        public string Adresvoorstelling { get; set; } = null!;
        public Adres? Adres { get; set; }
        public string? Naam { get; set; }
        public AdresId? AdresId { get; set; }
        public class LocatieType
        {
            public JsonLdMetadata JsonLdMetadata { get; set; }
            public string Naam { get; set; }

        }
    }

    public class HoofdactiviteitVerenigingsloket
    {
        public JsonLdMetadata JsonLdMetadata { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Naam { get; set; } = null!;
    }

    public class Sleutel
    {
        public JsonLdMetadata JsonLdMetadata { get; set; } = null!;
        public string Bron { get; set; } = null!;
        public string Waarde { get; set; } = null!;
        public GestructureerdeIdentificator GestructureerdeIdentificator { get; set; }
    }

    public class GestructureerdeIdentificator
    {
        public JsonLdMetadata JsonLdMetadata { get; set; } = null!;
        public string Nummer { get; set; } = null!;
    }

    public class Relatie
    {
        public string Relatietype { get; set; } = null!;
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
        public JsonLdMetadata JsonLdMetadata { get; set; } = null!;
        public string Straatnaam { get; init; } = null!;
        public string Huisnummer { get; init; } = null!;
        public string? Busnummer { get; init; }
        public string Postcode { get; init; } = null!;
        public string Gemeente { get; init; } = null!;
        public string Land { get; init; } = null!;
    }

    public bool Deleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}

public class JsonLdMetadata
{
    public JsonLdMetadata()
    {
    }

    public JsonLdMetadata(string id, string type)
    {
        Id = id;
        Type = type;
    }

    public string Id { get; set; }
    public string Type { get; set; }
}
