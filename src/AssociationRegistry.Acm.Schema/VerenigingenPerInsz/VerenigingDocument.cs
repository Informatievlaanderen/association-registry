namespace AssociationRegistry.Acm.Schema.VerenigingenPerInsz;

using Marten.Schema;

public class VerenigingDocument
{
    [Identity]
    public string VCode { get; set; } = null!;

    public string Naam { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string KboNummer { get; set; } = null!;
}
