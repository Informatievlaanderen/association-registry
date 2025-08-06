namespace AssociationRegistry.Public.Api.WebApi.Verenigingen.Mutaties;

using System.Runtime.Serialization;

[DataContract]
public class PubliekVerenigingSequenceResponse
{
    /// <summary> De vcode van de vereniging</summary>
    [DataMember(Name = "vCode")]
    public string VCode { get; init; } = null!;

    /// <summary> De sequence van de vereniging</summary>
    [DataMember(Name = "sequence")]
    public long Sequence { get; init; }
}
