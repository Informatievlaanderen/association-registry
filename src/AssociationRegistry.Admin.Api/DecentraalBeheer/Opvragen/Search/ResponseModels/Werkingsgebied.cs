namespace AssociationRegistry.Admin.Api.Verenigingen.Search.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class Werkingsgebied
{
    /// <summary>De json-ld id</summary>
    [DataMember(Name = "@id")]
    public string id { get; init; }

    /// <summary>Het json-ld type</summary>
    [DataMember(Name = "@type")]
    public string type { get; set; }

    /// <summary>
    /// De code van het werkingsgebied
    /// </summary>
    [DataMember]
    public string Code { get; set; } = null!;

    /// <summary>
    /// De volledige beschrijving van het werkingsgebied
    /// </summary>
    [DataMember]
    public string Naam { get; set; } = null!;
}
