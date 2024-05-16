namespace AssociationRegistry.Admin.Schema.Detail;

using Marten.Schema;

public record LocatieLookupDocument : IVCode, IMetadata
{
    [Identity]
    public string VCode { get; init; } = null!;

    public LocatieLookupEntry[] Locaties { get; set; } = Array.Empty<LocatieLookupEntry>();
    public string DatumLaatsteAanpassing { get; set; } = null!;
    public Metadata Metadata { get; set; } = null!;
}

public record LocatieLookupEntry
{
    public int LocatieId { get; set; }
    public string AdresId { get; set; }
}
