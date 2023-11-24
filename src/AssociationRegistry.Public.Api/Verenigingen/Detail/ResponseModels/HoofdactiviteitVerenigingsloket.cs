namespace AssociationRegistry.Public.Api.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class HoofdactiviteitVerenigingsloket
{
    /// <summary>De code van de hoofdactivititeit</summary>
    [DataMember(Name = "Code")]
    public string Code { get; init; } = null!;

    /// <summary>De beschrijving van de hoofdactivititeit</summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; init; }= null!;
}
