namespace AssociationRegistry.Acm.Schema.VerenigingenPerInsz;

using Marten.Schema;

public class VerenigingenPerInszDocument
{
    /// <summary>
    /// Het Insz nummer van de aanvraag.
    /// </summary>
    [Identity] public string Insz { get; set; } = null!;

    /// <summary>
    /// De lijst van verenigingen die aan het opgegeven insz gekoppeld zijn.
    /// </summary>
    public List<Vereniging> Verenigingen { get; set; } = new();
}

public class Vereniging
{
    /// <summary>
    /// De vCode van de vereniging
    /// </summary>
    public string VCode { get; set; } = null!;

    /// <summary>
    /// De id van de vertegenwoordiger
    /// </summary>
    public int VertegenwoordigerId { get; set; }

    /// <summary>
    /// De naam van de vereniging
    /// </summary>
    public string Naam { get; set; } = null!;

    // <summary>
    /// De status van de vereniging
    /// </summary>
    public string Status { get; set; } = null!;

    // <summary>
    /// Het kbo nummer van de vereniging
    /// </summary>
    public string KboNummer { get; set; } = null!;

    public bool IsHoofdvertegenwoordigerVan { get; set; }
}
