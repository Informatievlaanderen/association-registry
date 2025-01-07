namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Search.ResponseModels;

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

    /// <summary>
    /// De vCode van de andere vereniging
    /// </summary>
    [DataMember]
    public string AndereVereniging { get; set; }

    /// <summary>
    /// De datum waarop het lidmaatschap actief is
    /// </summary>
    [DataMember]
    public string? Van { get; set; }

    /// <summary>
    /// De datum waarop het lidmaatschap niet meer actief is
    /// </summary>
    [DataMember]
    public string? Tot { get; set; }

    /// <summary>
    /// De externe identificatie voor het lidmaatschap
    /// </summary>
    [DataMember]
    public string? Identificatie { get; set; } = string.Empty;

    /// <summary>
    /// De externe beschrijving van het lidmaatschap
    /// </summary>
    [DataMember]
    public string? Beschrijving { get; set; } = string.Empty;
}
