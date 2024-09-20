namespace AssociationRegistry.Public.Api.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class PubliekVerenigingDetailResponse
{
    /// <summary>De JSON-LD open data context</summary>
    [DataMember(Name = "@context")]
    public string Context { get; set; } = null!;

    /// <summary> De vereniging waarvan het detail opgevraagd werd</summary>
    [DataMember(Name = "Vereniging")]
    public Vereniging Vereniging { get; init; } = null!;

    /// <summary>De metadata van de vereniging, deze bevat bv de datum van laatste aanpassing</summary>
    [DataMember(Name = "Metadata")]
    public Metadata Metadata { get; init; } = null!;
}
