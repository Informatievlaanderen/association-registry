namespace AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

/// <summary>De hoofdactivititeit van een vereniging volgens het verenigingsloket</summary>
[DataContract]
public class HoofdactiviteitVerenigingsloket
{
    /// <summary>De code van de hoofdactivititeit</summary>
    [DataMember(Name = "Code")]
    public string Code { get; init; } = null!;

    /// <summary>De beschrijving van de hoofdactivititeit</summary>
    [DataMember(Name = "Beschrijving")]
    public string Beschrijving { get; init; } = null!;
}