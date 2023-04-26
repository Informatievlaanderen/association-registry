namespace AssociationRegistry.Acm.Schema.VerenigingenPerInsz;

using Marten.Schema;

public class VerenigingDocument
{
    /// <summary>
    /// De vCode van de vereniging
    /// </summary>
    [Identity]
    public string VCode { get; set; } = null!;

    /// <summary>
    /// De naam van de vereniging
    /// </summary>
    public string Naam { get; set; } = null!;
}
