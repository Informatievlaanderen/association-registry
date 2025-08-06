namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Search.ResponseModels;

using System.Runtime.Serialization;

/// <summary>
/// In deze metadata plaatsen we alle relevante metadata voor de zoekopdracht, de paginering informatie
/// </summary>
[DataContract]
public class Metadata
{
    /// <summary>
    /// De paginatie metaData
    /// </summary>
    [DataMember(Name = "Pagination")]
    public Pagination Pagination { get; init; } = null!;
}
