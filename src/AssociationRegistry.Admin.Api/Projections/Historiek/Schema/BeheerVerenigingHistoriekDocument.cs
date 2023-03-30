namespace AssociationRegistry.Admin.Api.Projections.Historiek.Schema;

using System.Collections.Generic;
using Detail;
using Marten.Schema;

public class BeheerVerenigingHistoriekDocument : IMetadata, IVCode
{
    public List<BeheerVerenigingHistoriekGebeurtenis> Gebeurtenissen { get; set; } = null!;
    public Metadata Metadata { get; set; } = null!;
    [Identity] public string VCode { get; set; } = null!;
}
