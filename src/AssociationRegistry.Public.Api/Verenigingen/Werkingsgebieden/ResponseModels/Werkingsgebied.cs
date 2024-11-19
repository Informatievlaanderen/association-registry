namespace AssociationRegistry.Public.Api.Verenigingen.Werkingsgebieden.ResponseModels;

using Detail.ResponseModels;
using System.Runtime.Serialization;

[DataContract]
public class Werkingsgebied
{
    /// <summary>De code van het werkingsgebied</summary>
    [DataMember(Name = "Code")]
    public string Code { get; init; } = null!;

    /// <summary>De beschrijving van het werkingsgebied</summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; init; } = null!;
}
