namespace AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class SubtypeData
{
    /// <summary>De json-ld id</summary>
    [DataMember(Name = "@id")]
    public string id { get; init; }

    /// <summary>Het json-ld type</summary>
    [DataMember(Name = "@type")]
    public string type { get; set; }

    /// <summary>
    /// Het subtype van de vereniging
    /// </summary>
    [DataMember(Name = "Subtype")]
    public Subtype Subtype { get; set; } = null!;

    /// <summary>
    /// De unieke identificator van de vereniging waarvan deze vereniging een subvereniging werd
    /// </summary>
    [DataMember(Name = "AndereVereniging", EmitDefaultValue = false)]
    public string AndereVereniging { get; set; } = null!;

    /// <summary>
    /// De naam van de gerelateerde vereniging
    /// </summary>
    [DataMember(Name = "Naam", EmitDefaultValue = false)]
    public string Naam { get; set; } = null!;

    /// <summary>
    /// De identificatie van het subtype
    /// </summary>
    [DataMember(Name = "Identificatie", EmitDefaultValue = false)]
    public string Identificatie { get; set; } = string.Empty;

    /// <summary>
    /// De beschrijving van het subtype
    /// </summary>
    [DataMember(Name = "Beschrijving", EmitDefaultValue = false)]
    public string Beschrijving { get; set; } = string.Empty;
}

/// <summary>
/// Het subtype van de vereniging
/// </summary>
public class Subtype
{
    /// <summary>
    /// De code van het subtype
    /// </summary>
    [DataMember(Name = "Code")]
    public string Code { get; init; } = null!;

    /// <summary>
    /// De beschrijving van het subtype
    /// </summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; init; } = null!;
}
