namespace AssociationRegistry.Public.Api.WebApi.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class AdresVerwijzing
{
    /// <summary>De json-ld id</summary>
    [DataMember(Name = "@id")]
    public string id { get; init; }

    /// <summary>Het json-ld type</summary>
    [DataMember(Name = "@type")]
    public string type { get; init; }
}
