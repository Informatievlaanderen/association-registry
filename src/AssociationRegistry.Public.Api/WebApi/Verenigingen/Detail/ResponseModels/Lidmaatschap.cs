namespace AssociationRegistry.Public.Api.WebApi.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class Lidmaatschap
{
    /// <summary>De json-ld id</summary>
    [DataMember(Name = "@id")]
    public string id { get; init; }

    /// <summary>Het json-ld type</summary>
    [DataMember(Name = "@type")]
    public string type { get; set; }

    // /// <summary>
    // /// De unieke identificatie code van dit lidmaatschap binnen de vereniging
    // /// </summary>
    // [DataMember(Name = "LidmaatschapId")]
    // public int LidmaatschapId { get; set; }

    /// <summary>
    /// De unieke identificator van de vereniging waarvan deze vereniging lid werd
    /// </summary>
    [DataMember(Name = "AndereVereniging")]
    public string AndereVereniging { get; set; } = null!;

    /// <summary>
    /// De naam van de gerelateerde vereniging
    /// </summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; set; } = null!;

    /// <summary>
    /// De identificatie van het lidmaatschap
    /// </summary>
    [DataMember(Name = "Identificatie")]
    public string Identificatie { get; set; } = string.Empty;

    /// <summary>
    /// De beschrijving van het lidmaatschap
    /// </summary>
    [DataMember(Name = "Beschrijving")]
    public string Beschrijving { get; set; } = string.Empty;

    /// <summary>De datum waarop het lidmaatschap start</summary>
    [DataMember(Name = "Van")]
    public string Van { get; init; } = string.Empty;

    /// <summary>De datum waarop het lidmaatschap stopt</summary>
    [DataMember(Name = "Tot")]
    public string Tot { get; init; } = string.Empty;
}
