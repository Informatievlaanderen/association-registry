namespace AssociationRegistry.Public.Api.WebApi.Werkingsgebieden.ResponseModels;

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
