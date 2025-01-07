namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Search.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class Vereniging
{
    /// <summary>Het json-ld type</summary>
    [DataMember(Name = "@type")]
    public string type { get; set; }

    /// <summary>De vCode van de vereniging</summary>
    [DataMember(Name = "VCode")]
    public string VCode { get; init; } = null!;

    /// <summary>De vCodes van de corresponderende verenigingen</summary>
    [DataMember(Name = "CorresponderendeVCodes")]
    public string[] CorresponderendeVCodes { get; init; } = Array.Empty<string>()!;

    /// <summary>Het type van de vereniging</summary>
    [DataMember(Name = "Verenigingstype")]
    public VerenigingsType Verenigingstype { get; init; } = null!;

    /// <summary>De naam van de vereniging</summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; init; } = null!;

    /// <summary>Roepnaam van de vereniging. Dit veld is enkel aanwezig bij verenigingen met rechtspersoonlijkheid</summary>
    [DataMember(Name = "Roepnaam", EmitDefaultValue = false)]
    public string Roepnaam { get; init; } = null!;

    /// <summary>De korte naam van de vereniging</summary>
    [DataMember(Name = "KorteNaam")]
    public string KorteNaam { get; init; } = null!;

    /// <summary>Status van de vereniging<br />
    ///     <br />
    ///     Mogelijke waarden:<br />
    ///     - Actief<br />
    ///     - Gestopt<br />
    /// </summary>
    [DataMember(Name = "Status")]
    public string Status { get; init; } = null!;

    /// <summary>Datum waarop de vereniging gestart is</summary>
    [DataMember(Name = "Startdatum")]
    public string? Startdatum { get; init; }

    /// <summary>Datum waarop de vereniging gestopt is</summary>
    [DataMember(Name = "Einddatum")]
    public string? Einddatum { get; init; }

    /// <summary>De doelgroep waar de activiteiten van deze vereniging zich op concentreert</summary>
    [DataMember(Name = "Doelgroep")]
    public DoelgroepResponse Doelgroep { get; init; } = null!;

    /// <summary>De lijst van hoofdactiviteiten erkend door het vereningingsloket</summary>
    [DataMember(Name = "HoofdactiviteitenVerenigingsloket")]
    public HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; init; } = null!;

    /// <summary>De lijst van werkingsgebieden</summary>
    [DataMember(Name = "Werkingsgebieden")]
    public Werkingsgebied[] Werkingsgebieden { get; init; } = null!;

    /// <summary>De locaties waar de vereniging actief is</summary>
    [DataMember(Name = "Locaties")]
    public Locatie[] Locaties { get; init; } = null!;

    /// <summary>De sleutels van deze vereniging</summary>
    [DataMember(Name = "Sleutels")]
    public Sleutel[] Sleutels { get; init; } = null!;

    /// <summary>Weblinks i.v.m. deze vereniging</summary>
    [DataMember(Name = "Links")]
    public VerenigingLinks Links { get; init; } = null!;

    /// <summary>De lidmaatschappen van de vereniging</summary>
    [DataMember(Name = "Lidmaatschappen")]
    public Lidmaatschap[] Lidmaatschappen { get; init; } = null!;
}
