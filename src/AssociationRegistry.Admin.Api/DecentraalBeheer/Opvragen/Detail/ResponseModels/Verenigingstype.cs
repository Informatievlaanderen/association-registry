namespace AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;
using Vereniging;


/// <summary>
/// Het type van een vereniging
/// </summary>
[DataContract]
public class Verenigingstype : IVerenigingstype
{
    /// <summary>
    /// De code van het type vereniging
    /// </summary>
    [DataMember(Name = "Code")]
    public string Code { get; init; } = null!;

    /// <summary>
    /// De beschrijving van het type vereniging
    /// </summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; init; } = null!;
}
