namespace AssociationRegistry.Admin.Schema.PowerBiExport;

using Marten.Schema;

public record PowerBiExportDubbelDetectieDocument
{
    [Identity]
    public string Id { get; init; } = null!;
    public string Bevestigingstoken { get; init; } = null!;
    public string Naam { get; set; } = null!;
    public Types.Locatie[] Locaties { get; set; } = [];
    public Types.DuplicateVereniging[] GedetecteerdeDubbels { get; init; }
    public string Tijdstip { get; set; } = null!;
    public string Initiator { get; set; } = null!;
    public string CorrelationId { get; set; } = null!;

    public class Types
    {
        public record Locatie
        {
            public string Locatietype { get; set; } = null!;
            public bool IsPrimair { get; set; }
            public string Adresvoorstelling { get; init; } = null!;
            public Adres? Adres { get; set; }
            public string? Naam { get; set; }
            public AdresId? AdresId { get; set; }
            public string Bron { get; set; } = null!;
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

        public class AdresId
        {
            public string? Broncode { get; set; }
            public string? Bronwaarde { get; set; }
        }

        public record DuplicateVereniging
        {
            public string VCode { get; init; }
            public Verenigingstype Verenigingstype { get; init; }
            public Verenigingssubtype? Verenigingssubtype { get; init; }
            public string Naam { get; init; }
            public string KorteNaam { get; init; }
            public HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; init; }
            public DuplicateVerenigingLocatie[] Locaties { get; init; }
        }

        public record HoofdactiviteitVerenigingsloket
        {
            public string Code { get; init; } = null!;
            public string Naam { get; init; } = null!;
        }
        public record Verenigingstype(string Code, string Naam);

        public record Verenigingssubtype(string Code, string Naam);

        public record DuplicateVerenigingLocatie(
            string Locatietype,
            bool IsPrimair,
            string Adres,
            string? Naam,
            string Postcode,
            string Gemeente);
    }
}



