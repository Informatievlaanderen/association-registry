namespace AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;

using Schema;
using System.Runtime.Serialization;
using Vereniging;
using Vereniging.Verenigingstype;

/// <summary>
/// Het type van een vereniging
/// </summary>
[DataContract]
public class VerenigingsType : IVerenigingstype
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
