namespace AssociationRegistry.Admin.Api.Verenigingen.Search.ResponseModels;

using Schema.Detail;
using System.Runtime.Serialization;

[DataContract]
public class VerenigingsType : IVerenigingsType
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
