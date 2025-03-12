namespace AssociationRegistry.Public.Api.Verenigingen.Search.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class VerenigingssubtypeData
{
    /// <summary>
    /// Het subtype van de vereniging
    /// </summary>
    [DataMember(Name = "Subtype")]
    public Verenigingssubtype Verenigingssubtype { get; set; } = null!;

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
    /// De beschrijving van het subtype vereniging
    /// </summary>
    [DataMember(Name = "Beschrijving", EmitDefaultValue = false)]
    public string Beschrijving { get; set; } = string.Empty;
}
