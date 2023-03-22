namespace AssociationRegistry.Admin.Api.Projections.Historiek.Schema;

using System.Collections.Generic;
using AssociationRegistry.Admin.Api.Projections.Detail;
using Marten.Schema;

public class BeheerVerenigingHistoriekDocument : IMetadata, IVCode
{
    [Identity] public string VCode { get; set; } = null!;
    public List<BeheerVerenigingHistoriekGebeurtenis> Gebeurtenissen { get; set; } = null!;
    public Metadata Metadata { get; set; } = null!;
}
