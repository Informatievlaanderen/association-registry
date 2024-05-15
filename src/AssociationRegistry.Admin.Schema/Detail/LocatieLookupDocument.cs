namespace AssociationRegistry.Admin.Schema.Detail;

using Marten.Schema;

public record LocatieLookupDocument : IVCode, IMetadata
{
    [Identity]
    public string Key { get; init; }

    public string VCode { get; init; } = null!;
    public int LocatieId { get; set; }
    public string AdresId { get; set; }
    public string DatumLaatsteAanpassing { get; set; } = null!;
    public Metadata Metadata { get; set; } = null!;

    public static string GetKey(string vCode, int locatieId)
        => $"{vCode}_{locatieId}";
}
