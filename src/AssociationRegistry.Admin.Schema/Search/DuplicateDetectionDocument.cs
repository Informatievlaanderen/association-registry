namespace AssociationRegistry.Admin.Schema.Search;

// Use this for updates when properties are not nullable so it does not get overwritten on an updatepublic
public record DuplicateDetectionUpdateDocument
{
    public string VCode { get; set; } = null!;
    public string Naam { get; set; } = null!;
    public Locatie[] Locaties { get; set; } = null!;
    public string VerenigingsTypeCode { get; set; } = null!;
    public string? VerenigingssubtypeCode { get; set; } = null!;
    public string KorteNaam { get; set; } = null!;
    public string[] HoofdactiviteitVerenigingsloket { get; set; } = null!;
    public bool? IsGestopt { get; set; } = null!;
    public bool? IsVerwijderd { get; set; } = null!;
    public bool? IsDubbel { get; set; } = null!;

    public record Locatie : ILocatie
    {
        public int LocatieId { get; init; }
        public string Locatietype { get; init; } = null!;
        public string? Naam { get; init; }
        public string Adresvoorstelling { get; init; } = null!;
        public bool? IsPrimair { get; init; } = null!;
        public string Postcode { get; init; } = null!;
        public string Gemeente { get; init; } = null!;
    }
}

public record DuplicateDetectionDocument
{
    public string VCode { get; set; } = null!;
    public string Naam { get; set; } = null!;
    public Locatie[] Locaties { get; set; } = null!;
    public string VerenigingsTypeCode { get; set; } = null!;
    public string? VerenigingssubtypeCode { get; set; } = null!;
    public string KorteNaam { get; set; } = null!;
    public string[] HoofdactiviteitVerenigingsloket { get; set; } = null!;
    public bool IsGestopt { get; set; }
    public bool IsVerwijderd { get; set; }
    public bool IsDubbel { get; set; }

    public record Locatie : ILocatie
    {
        public int LocatieId { get; init; }
        public string Locatietype { get; init; } = null!;
        public string? Naam { get; init; }
        public string Adresvoorstelling { get; init; } = null!;
        public bool IsPrimair { get; init; }
        public string Postcode { get; init; } = null!;
        public string Gemeente { get; init; } = null!;
    }
}
