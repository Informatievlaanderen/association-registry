namespace AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

/// <summary>De werkingsgebied van een vereniging</summary>
[DataContract]
public class Werkingsgebied
{
    /// <summary>De json-ld id</summary>
    [DataMember(Name = "@id")]
    public string id { get; init; }

    /// <summary>Het json-ld type</summary>
    [DataMember(Name = "@type")]
    public string type { get; init; }

    /// <summary>De code van het werkingsgebied</summary>
    [DataMember(Name = "Code")]
    public string Code { get; init; } = null!;

    /// <summary>De beschrijving van het werkingsgebied</summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; init; } = null!;
}
