namespace AssociationRegistry.Public.Api.Verenigingen.Search.ResponseModels;

using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Verenigingstype;
using System.Runtime.Serialization;

[DataContract]
public class VerenigingsType : IVerenigingstype
{
    /// <summary>
    /// De code van het type vereniging
    /// </summary>
    [DataMember]
    public string Code { get; init; } = null!;

    /// <summary>
    /// De beschrijving van het type vereniging
    /// </summary>
    [DataMember]
    public string Naam { get; init; } = null!;
}
