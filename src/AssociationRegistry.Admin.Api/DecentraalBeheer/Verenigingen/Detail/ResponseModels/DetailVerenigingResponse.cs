namespace AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels;

using System.Runtime.Serialization;

[DataContract]
public class DetailVerenigingResponse
{
    /// <summary>De JSON-LD open data context</summary>
    [DataMember(Name = "@context")]
    public string Context { get; init; } = null!;

    [DataMember(Name = "Vereniging")] public VerenigingDetail Vereniging { get; init; } = null!;
    [DataMember(Name = "Metadata")] public Metadata Metadata { get; init; } = null!;
}
