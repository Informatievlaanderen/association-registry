namespace AssociationRegistry.Public.Api.Verenigingen.Detail.ResponseModels;

using AssociationRegistry.Vereniging;
using System.Runtime.Serialization;

[DataContract]
public class Verenigingstype : IVerenigingstype
{
    /// <summary>
    ///     Code van het type vereniging
    /// </summary>
    [DataMember(Name = "Code")]
    public string Code { get; init; } = null!;

    /// <summary>
    ///     Beschrijving van het type vereniging
    /// </summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; init; } = null!;
}
