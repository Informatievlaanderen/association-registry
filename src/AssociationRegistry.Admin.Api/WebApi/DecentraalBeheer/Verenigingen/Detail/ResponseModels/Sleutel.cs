namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

/// <summary>
/// Een uniek identificerende sleutel van deze vereniging in een externe bron
/// </summary>
[DataContract]
public class Sleutel
{
    /// <summary>De json-ld id</summary>
    [DataMember(Name = "@id")]
    public string id { get; init; }

    /// <summary>Het json-ld type</summary>
    [DataMember(Name = "@type")]
    public string type { get; init; }

    /// <summary>
    /// De bron van de sleutel
    /// </summary>
    [DataMember(Name = "Bron")]
    public string Bron { get; set; } = null!;

    /// <summary>
    /// De externe identificator van de vereniging in de bron
    /// </summary>
    [DataMember(Name = "Waarde")]
    public string Waarde { get; set; } = null!;

    /// <summary>
    /// het codeersysteem van de sleutel
    /// </summary>
    [DataMember(Name = "CodeerSysteem")]
    public string CodeerSysteem { get; set; } = null!;

    /// <summary>De gestructureerde identificator</summary>
    [DataMember(Name = "GestructureerdeIdentificator")]
    public GestructureerdeIdentificator GestructureerdeIdentificator { get; set; }
}
