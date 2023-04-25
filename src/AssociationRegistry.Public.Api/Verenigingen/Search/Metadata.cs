namespace AssociationRegistry.Public.Api.Verenigingen.Search;

using System.Runtime.Serialization;
using Constants;

/// <summary>
/// In deze metadata plaatsen we alle relevante metadata voor de zoekopdracht, de paginering informatie.
/// </summary>
[DataContract]
public class Metadata
{
    /// <summary>
    /// De paginatie metaData.
    /// </summary>
    [DataMember]
    public Pagination Pagination { get; init; } = null!;
}

/// <summary>
/// De paginatie metaData.
/// </summary>
[DataContract]
public class Pagination
{
    /// <summary>
    /// het totaal aantal verenigingen met de huidige limit.
    /// </summary>
    [DataMember]
    public long TotalCount { get; init; }

    /// <summary>
    /// Het aantal overgeslagen resultaten.
    /// </summary>
    [DataMember]
    public int Offset { get; init; }

    /// <summary>
    /// Het maximum aantal teruggegeven resultaten.
    /// </summary>
    [DataMember]
    public int Limit { get; init; }
}

/// <summary>
/// De paginatie parameters
/// </summary>
[DataContract]
public class PaginationQueryParams
{
    /// <summary>
    /// Het aantal items dat overgeslagen zal worden
    /// </summary>
    [DataMember]
    public int Offset { get; set; } = PagingConstants.DefaultOffset;

    /// <summary>
    /// Het aantal items dat (maximaal) zal worden opgehaald.
    /// </summary>
    /// <remarks>
    /// De laatste pagina kan minder items bevatten.
    /// </remarks>
    [DataMember]
    public int Limit { get; set; } = PagingConstants.DefaultLimit;
}
