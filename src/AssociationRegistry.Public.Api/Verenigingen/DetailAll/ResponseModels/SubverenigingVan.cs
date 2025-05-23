namespace AssociationRegistry.Public.Api.Verenigingen.DetailAll.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class SubverenigingVan
{
    /// <summary>
    /// De unieke identificator van de vereniging waarvan deze vereniging een subvereniging werd
    /// </summary>
    [DataMember(Name = "AndereVereniging", EmitDefaultValue = false)]
    public string AndereVereniging { get; set; } = null!;

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
