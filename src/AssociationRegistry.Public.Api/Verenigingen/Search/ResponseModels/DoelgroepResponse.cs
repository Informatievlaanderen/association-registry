namespace AssociationRegistry.Public.Api.Verenigingen.Search.ResponseModels;

using System.Runtime.Serialization;

/// <summary>
/// De doelgroep waar de activiteiten van deze vereniging zich op concentreert
/// </summary>
[DataContract]
public class DoelgroepResponse
{
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
