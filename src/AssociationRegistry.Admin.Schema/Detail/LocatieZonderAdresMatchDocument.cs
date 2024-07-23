namespace AssociationRegistry.Admin.Schema.Detail;

public record LocatieZonderAdresMatchDocument : IVCode, IMetadata
{
    public string Id { get; set; }
    public string VCode { get; set; } = null!;
    public int LocatieId { get; set; }

    public Metadata Metadata { get; set; } = null!;
}
