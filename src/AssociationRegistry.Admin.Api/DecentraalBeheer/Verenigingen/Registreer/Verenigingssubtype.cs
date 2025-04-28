namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using System.Runtime.Serialization;
using Vereniging;

/// <summary>
/// Het subtype van de vereniging
/// </summary>
public class Verenigingssubtype : IVerenigingssubtypeCode
{
    /// <summary>
    /// De code van het subtype vereniging
    /// </summary>
    [DataMember(Name = "Code")]
    public string Code { get; init; } = null!;

    /// <summary>
    /// De beschrijving van het subtype vereniging
    /// </summary>
    [DataMember(Name = "Naam")]
    public string Naam { get; init; } = null!;
}
