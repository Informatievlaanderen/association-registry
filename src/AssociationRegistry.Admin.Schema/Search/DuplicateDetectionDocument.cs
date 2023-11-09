namespace AssociationRegistry.Admin.Schema.Search;

public class DuplicateDetectionDocument
{
    public string VCode { get; set; } = null!;
    public string Naam { get; set; } = null!;
    public VerenigingZoekDocument.Locatie[] Locaties { get; set; } = null!;
}
