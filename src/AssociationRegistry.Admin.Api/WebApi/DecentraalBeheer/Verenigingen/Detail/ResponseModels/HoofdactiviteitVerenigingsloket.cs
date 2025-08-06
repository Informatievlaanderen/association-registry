namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

/// <summary>De hoofdactivititeit van een vereniging volgens het verenigingsloket</summary>
[DataContract]
public class HoofdactiviteitVerenigingsloket
{
    /// <summary>De json-ld id</summary>
    [DataMember(Name = "@id")]
    public string id { get; init; }

    /// <summary>Het json-ld type</summary>
    [DataMember(Name = "@type")]
    public string type { get; init; }

    /// <summary>De code van de hoofdactivititeit</summary>
    [DataMember(Name = "Code")]
    public string Code { get; init; } = null!;

    /// <summary>De beschrijving van de hoofdactivititeit</summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; init; } = null!;
}
