namespace AssociationRegistry.Admin.Schema.Search;

public class VerenigingZoekDocument
{
    public string JsonLdMetadataType { get; set; }
    public string VCode { get; set; } = null!;

    public string[] CorresponderendeVCodes { get; set; } = Array.Empty<string>();
    public VerenigingsType Verenigingstype { get; set; } = null!;
    public string Naam { get; set; } = null!;
    public string Roepnaam { get; set; } = null!;
    public string KorteNaam { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? Startdatum { get; set; } = null!;
    public string? Einddatum { get; set; } = null!;
    public Doelgroep Doelgroep { get; set; } = null!;
    public Locatie[] Locaties { get; set; } = null!;
    public HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; set; } = null!;
    public Sleutel[] Sleutels { get; set; } = null!;
    public bool IsUitgeschrevenUitPubliekeDatastroom { get; set; }
    public bool IsVerwijderd { get; set; }

    public class Locatie : ILocatie
    {
        public JsonLdMetadata JsonLdMetadata { get; set; }
        public int LocatieId { get; init; }
        public string Locatietype { get; init; } = null!;
        public string? Naam { get; init; }
        public string Adresvoorstelling { get; init; } = null!;
        public bool IsPrimair { get; init; }
        public string Postcode { get; init; } = null!;
        public string Gemeente { get; init; } = null!;

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

    public class VerenigingsType
    {
        public string Code { get; init; } = null!;
        public string Naam { get; init; } = null!;
    }

    public class Sleutel
    {
        public JsonLdMetadata JsonLdMetadata { get; set; } = null!;
        public string Bron { get; set; } = null!;
        public string Waarde { get; set; } = null!;
        public GestructureerdeIdentificator GestructureerdeIdentificator { get; set; }
        public string CodeerSysteem { get; set; }
    }

    public class GestructureerdeIdentificator
    {
        public JsonLdMetadata JsonLdMetadata { get; set; } = null!;
        public string Nummer { get; set; } = null!;
    }
}

public class Doelgroep
{
    public JsonLdMetadata JsonLdMetadata { get; set; }
    public int Minimumleeftijd { get; set; }
    public int Maximumleeftijd { get; set; }
}
