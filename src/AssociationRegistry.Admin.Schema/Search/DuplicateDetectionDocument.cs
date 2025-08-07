namespace AssociationRegistry.Admin.Schema.Search;

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
        public int LocatieId { get; set; }
        public string Locatietype { get; set; } = null!;
        public string? Naam { get; set; }
        public string Adresvoorstelling { get; set; } = null!;
        public bool IsPrimair { get; set; }
        public string Postcode { get; set; } = null!;
        public string Gemeente { get; set; } = null!;
    }
}
