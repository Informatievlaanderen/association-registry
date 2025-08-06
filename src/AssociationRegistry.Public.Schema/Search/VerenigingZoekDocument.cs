namespace AssociationRegistry.Public.Schema.Search;

using DecentraalBeheer.Vereniging;
using Detail;
using Vereniging;

public class VerenigingZoekDocument : ICanBeUitgeschrevenUitPubliekeDatastroom, IHasStatus, IDeletable, IIsDubbel
{
    public string JsonLdMetadataType { get; set; }
    public string VCode { get; set; } = null!;
    public Types.Verenigingstype Verenigingstype { get; set; } = null!;
    public Types.Verenigingssubtype? Verenigingssubtype { get; set; }
    public Types.SubverenigingVan? SubverenigingVan { get; set; }
    public string Naam { get; set; } = null!;
    public string? Roepnaam { get; set; }
    public string KorteNaam { get; set; } = null!;
    public string KorteBeschrijving { get; set; } = null!;
    public Types.Doelgroep Doelgroep { get; set; } = null!;
    public Types.Locatie[] Locaties { get; set; } = null!;
    public Types.HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; set; } = null!;
    public Types.Werkingsgebied[] Werkingsgebieden { get; set; } = null!;
    public Types.Lidmaatschap[] Lidmaatschappen { get; set; } = null!;
    public Types.Sleutel[] Sleutels { get; set; } = null!;
    public Types.Relatie[] Relaties { get; set; } = null!;
    public bool? IsUitgeschrevenUitPubliekeDatastroom { get; set; }
    public string Status { get; set; } = null!;
    public bool IsVerwijderd { get; set; }
    public bool IsDubbel { get; set; }
    public Types.Geotag[] Geotags { get; set; } = null!;

    public static class Types
    {
        public record Geotag(string Identificatie);

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

        public class Locatie
        {
            public JsonLdMetadata JsonLdMetadata { get; set; }
            public string Locatietype { get; set; } = null!;
            public string? Naam { get; set; }
            public string Adresvoorstelling { get; set; } = null!;
            public bool IsPrimair { get; set; }
            public string Postcode { get; set; } = null!;
            public string Gemeente { get; set; } = null!;
            public int LocatieId { get; set; }

            public class LocatieType
            {
                public JsonLdMetadata JsonLdMetadata { get; set; }
                public string Naam { get; set; }
            }
        }

        public class Lidmaatschap
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

        public class Verenigingstype : IVerenigingstype
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
    }
}

