namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Search.ResponseModels;

using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging;
using System.Runtime.Serialization;

[DataContract]
public class Verenigingstype : IVerenigingstype
{
    /// <summary>
    /// De code van het type vereniging
    /// </summary>
    [DataMember (Name = "code")]
    public string Code { get; init; } = null!;

    /// <summary>
    /// De beschrijving van het type vereniging
    /// </summary>
    [DataMember (Name = "naam")]
    public string Naam { get; init; } = null!;
}
