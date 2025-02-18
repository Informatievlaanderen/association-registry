namespace AssociationRegistry.Public.Schema.Sequence;

using Marten.Metadata;
using Marten.Schema;

public class PubliekVerenigingSequenceDocument : IRevisioned
{
    [Identity]
    public string VCode { get; set; }
    public long Sequence { get; set; }
    public int Version { get; set; }
}
