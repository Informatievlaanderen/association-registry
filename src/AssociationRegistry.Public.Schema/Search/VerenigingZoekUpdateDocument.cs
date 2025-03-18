namespace AssociationRegistry.Public.Schema.Search;

public class VerenigingZoekUpdateDocument
{
    public string VCode { get; set; } = null!;
    public VerenigingZoekDocument.Types.Verenigingstype Verenigingstype { get; set; } = null!;
    public VerenigingZoekDocument.Types.Verenigingssubtype? Verenigingssubtype { get; set; }
    public bool? IsVerwijderd { get; set; }
    public bool? IsDubbel { get; set; }
}
