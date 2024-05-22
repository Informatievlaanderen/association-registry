namespace AssociationRegistry.Admin.Schema.Detail;

public record LocatieLookupDocument : IVCode, IMetadata
{
    public string Id { get; set; }
    public string VCode { get; init; } = null!;
    public int LocatieId { get; set; }
    public string AdresId { get; set; }
    public string DatumLaatsteAanpassing { get; set; } = null!;
    public Metadata Metadata { get; set; } = null!;
}

public record LocatieLookupEntry
{
    public int LocatieId { get; set; }
    public string AdresId { get; set; }
}
