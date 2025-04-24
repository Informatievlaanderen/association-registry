namespace AssociationRegistry.Admin.Schema.Detail;

using Marten.Schema;

public record LocatieZonderAdresMatchDocument : IVCode, IMetadata
{
    [Identity]
    public string VCode { get; set; } = null!;

    public int[] LocatieIds { get; set; } = [];

    public Metadata Metadata { get; set; } = null!;
}
