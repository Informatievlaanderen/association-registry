namespace AssociationRegistry.Admin.Schema.Search;

using Vereniging;

public class VerenigingZoekDocument
{
    public string JsonLdMetadataType { get; set; }
    public string VCode { get; set; } = null!;

    public string[] CorresponderendeVCodes { get; set; } = null!;
    public Types.VerenigingsType Verenigingstype { get; set; } = null!;
    public Types.Verenigingssubtype? Verenigingssubtype { get; set; } = null!;
    public Types.SubverenigingVan? SubverenigingVan { get; set; }
    public string Naam { get; set; } = null!;
    public string Roepnaam { get; set; } = null!;
    public string KorteNaam { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? Startdatum { get; set; } = null!;
    public string? Einddatum { get; set; } = null!;
    public Types.Doelgroep Doelgroep { get; set; } = null!;
    public Types.Locatie[] Locaties { get; set; } = null!;
    public Types.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; set; } = null!;
    public Types.Werkingsgebied[] Werkingsgebieden { get; set; } = null!;
    public Types.Sleutel[] Sleutels { get; set; } = null!;
    public Types.Lidmaatschap[] Lidmaatschappen { get; set; } = null!;
    public bool? IsUitgeschrevenUitPubliekeDatastroom { get; set; }
    public bool IsVerwijderd { get; set; }
    public bool IsDubbel { get; set; }
    public Types.Geotag[] Geotags { get; set; } = null!;

    public static class Types
    {
        public record Geotag(string Identificatie);

        public class Locatie : ILocatie
        {
            public JsonLdMetadata JsonLdMetadata { get; set; }
            public int LocatieId { get; set; }
            public string Locatietype { get; set; } = null!;
            public string? Naam { get; set; }
            public string Adresvoorstelling { get; set; } = null!;
            public bool IsPrimair { get; set; }
            public string Postcode { get; set; } = null!;
            public string Gemeente { get; set; } = null!;

            public class LocatieType
            {
                public JsonLdMetadata JsonLdMetadata { get; set; }
                public string Naam { get; set; }
            }
        }

        public class Lidmaatschap : ILidmaatschap
        {
            public JsonLdMetadata JsonLdMetadata { get; set; }
            public int LidmaatschapId { get; set; }
            public string AndereVereniging { get; set; }
            public string DatumVan { get; set; }
            public string DatumTot { get; set; }
            public string Beschrijving { get; set; } = null!;
            public string Identificatie { get; set; } = null!;
        }

        public class HoofdactiviteitVerenigingsloket
        {
            public JsonLdMetadata JsonLdMetadata { get; set; }
            public string Code { get; set; } = null!;
            public string Naam { get; set; } = null!;
        }

        public class Werkingsgebied
        {
            public JsonLdMetadata JsonLdMetadata { get; set; }
            public string Code { get; set; } = null!;
            public string Naam { get; set; } = null!;
        }

        public class VerenigingsType : IVerenigingstype
        {
            public string Code { get; init; } = null!;
            public string Naam { get; init; } = null!;
        }

        public class Verenigingssubtype : IVerenigingssubtypeCode
        {
            public string Code { get; init; } = null!;
            public string Naam { get; init; } = null!;
        }

        public record SubverenigingVan
        {
            public string AndereVereniging { get; set; } = null!;
            public string Identificatie { get; set; } = null!;
            public string Beschrijving { get; set; } = null!;
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

        public class Doelgroep
        {
            public JsonLdMetadata JsonLdMetadata { get; set; }
            public int Minimumleeftijd { get; set; }
            public int Maximumleeftijd { get; set; }
        }

    }
}
