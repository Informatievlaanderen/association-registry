namespace AssociationRegistry.Admin.Schema.Historiek;

using Marten.Schema;

public class BeheerVerenigingHistoriekDocument : IMetadata, IVCode
{
    public List<BeheerVerenigingHistoriekGebeurtenis> Gebeurtenissen { get; set; } = new();
    public Metadata Metadata { get; set; } = null!;
    [Identity] public string VCode { get; set; } = null!;
}
