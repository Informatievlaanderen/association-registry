namespace AssociationRegistry.Acm.Schema.VerenigingenPerInsz;

using Marten.Metadata;
using Marten.Schema;

public class VerenigingDocument : ISoftDeleted
{
    [Identity]
    public string VCode { get; set; } = null!;

    public string Naam { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string KboNummer { get; set; } = null!;

    public Verenigingstype VerenigingsType { get; set; } = null;
    public bool Deleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public string[] CorresponderendeVCodes { get; set; } = [];
}
