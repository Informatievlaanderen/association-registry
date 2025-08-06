namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Search.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class HoofdactiviteitVerenigingsloket
{
    /// <summary>De json-ld id</summary>
    [DataMember(Name = "@id")]
    public string id { get; init; }

    /// <summary>Het json-ld type</summary>
    [DataMember(Name = "@type")]
    public string type { get; set; }

    /// <summary>
    /// De verkorte code van de hoofdactiviteit
    /// </summary>
    [DataMember]
    public string Code { get; set; } = null!;

    /// <summary>
    /// De volledige beschrijving van de hoofdactiviteit
    /// </summary>
    [DataMember]
    public string Naam { get; set; } = null!;
}
