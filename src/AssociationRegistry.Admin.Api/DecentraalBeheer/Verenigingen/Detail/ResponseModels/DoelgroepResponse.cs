namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

/// <summary>
/// De doelgroep waar de activiteiten van deze vereniging zich op concentreert
/// </summary>
[DataContract]
public class DoelgroepResponse
{
    /// <summary>De json-ld id</summary>
    [DataMember(Name = "@id")]
    public string id { get; init; }

    /// <summary>Het json-ld type</summary>
    [DataMember(Name = "@type")]
    public string type { get; init; }

    /// <summary>
    /// De minimum leeftijd voor de doelgroep
    /// </summary>
    [DataMember]
    public int Minimumleeftijd { get; set; }

    /// <summary>
    /// De maximum leeftijd voor de doelgroep
    /// </summary>
    [DataMember]
    public int Maximumleeftijd { get; set; }
}
