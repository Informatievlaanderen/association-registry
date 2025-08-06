namespace AssociationRegistry.Acm.Schema.VerenigingenPerInsz;

using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging;
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
    /// De vCodes van de corresponderende verenigingen
    /// </summary>
    public string[] CorresponderendeVCodes { get; set; } = Array.Empty<string>();

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

    // <summary>
    /// Het type van de vereniging
    /// </summary>
    public Verenigingstype Verenigingstype { get; set; } = null!;

    // <summary>
    /// Het subtype van de vereniging
    /// </summary>
    public Verenigingssubtype? Verenigingssubtype { get; set; } = null!;

    public bool IsHoofdvertegenwoordigerVan { get; set; }
    public bool IsDubbel { get; set; }
}

public class Verenigingstype
{
    public Verenigingstype(string code, string naam)
    {
        Code = code;
        Naam = naam;
    }

    /// <summary>
    /// De code van het type van de vereniging
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// De naam van het type van de vereniging
    /// </summary>
    public string Naam { get; set; } = null!;
}

public class Verenigingssubtype : IVerenigingssubtypeCode
{

    /// <summary>
    /// De code van het subtype vereniging van de vereniging
    /// </summary>
    public string Code { get; init; } = null!;

    /// <summary>
    /// De naam van het subtype van de vereniging
    /// </summary>
    public string Naam { get; init; } = null!;
}


