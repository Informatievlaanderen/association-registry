namespace AssociationRegistry.Admin.Schema.Relatieaanvragen;

using JasperFx;

public class RelatieaanvraagDocument : IMetadata
{
    [Identity]
    public string AanvraagId { get; set; } = null!;

    public string KoepelVCode { get; set; } = null!;
    public string VerenigingVCode { get; set; } = null!;
    public string AangevraagdDoor { get; set; } = null!;
    public string RelatieType { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateOnly AanvraagDatum { get; set; }
    public DateOnly VervalDatum { get; set; }
    public string Beschrijving { get; set; } = string.Empty;
    public Metadata Metadata { get; set; } = null!;
}
