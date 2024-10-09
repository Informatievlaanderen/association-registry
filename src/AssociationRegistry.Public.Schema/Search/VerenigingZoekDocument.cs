namespace AssociationRegistry.Public.Schema.Search;

using Detail;

public class VerenigingZoekDocument : ICanBeUitgeschrevenUitPubliekeDatastroom, IHasStatus, IDeletable
{
    public string JsonLdMetadataType { get; set; }
    public string VCode { get; set; } = null!;
    public VerenigingsType Verenigingstype { get; set; } = null!;
    public string Naam { get; set; } = null!;
    public string? Roepnaam { get; set; }
    public string KorteNaam { get; set; } = null!;
    public string KorteBeschrijving { get; set; } = null!;
    public Doelgroep Doelgroep { get; set; } = null!;
    public Locatie[] Locaties { get; set; } = null!;
    public HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; set; } = null!;
    public Werkingsgebied[] Werkingsgebieden { get; init; } = null!;
    public Sleutel[] Sleutels { get; set; } = null!;
    public Relatie[] Relaties { get; set; } = null!;
    public bool? IsUitgeschrevenUitPubliekeDatastroom { get; set; }
    public string Status { get; set; } = null!;
    public bool IsVerwijderd { get; set; }

    public class Locatie
    {
        public JsonLdMetadata JsonLdMetadata { get; set; }
        public string Locatietype { get; init; } = null!;
        public string? Naam { get; init; }
        public string Adresvoorstelling { get; init; } = null!;
        public bool IsPrimair { get; init; }
        public string Postcode { get; init; } = null!;
        public string Gemeente { get; init; } = null!;
        public int LocatieId { get; set; }

        public class LocatieType
        {
            public JsonLdMetadata JsonLdMetadata { get; set; }
            public string Naam { get; set; }
        }
    }

    public class HoofdactiviteitVerenigingsloket
    {
        public JsonLdMetadata JsonLdMetadata { get; set; }
        public string Code { get; init; } = null!;
        public string Naam { get; init; } = null!;
    }

    public class Werkingsgebied
    {
        public JsonLdMetadata JsonLdMetadata { get; set; }
        public string Code { get; init; } = null!;
        public string Naam { get; init; } = null!;
    }

    public class VerenigingsType
    {
        public string Code { get; init; } = null!;
        public string Naam { get; init; } = null!;
    }

    public class Sleutel
    {
        public JsonLdMetadata JsonLdMetadata { get; set; }
        public string Bron { get; set; } = null!;
        public string Waarde { get; set; } = null!;
        public string CodeerSysteem { get; set; } = null!;
        public GestructureerdeIdentificator GestructureerdeIdentificator { get; set; }
    }

    public class GestructureerdeIdentificator
    {
        public JsonLdMetadata JsonLdMetadata { get; set; }
        public string Nummer { get; set; } = null!;
    }
}

public class Doelgroep
{
    public JsonLdMetadata JsonLdMetadata { get; set; }
    public int Minimumleeftijd { get; set; }
    public int Maximumleeftijd { get; set; }
}

public class Relatie
{
    public string Relatietype { get; set; } = null!;
    public GerelateerdeVereniging AndereVereniging { get; set; } = null!;
}

public class GerelateerdeVereniging
{
    public string KboNummer { get; set; } = null!;
    public string VCode { get; set; } = null!;
    public string Naam { get; set; } = null!;
}
