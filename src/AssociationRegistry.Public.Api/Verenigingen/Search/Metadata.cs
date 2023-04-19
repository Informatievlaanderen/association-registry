namespace AssociationRegistry.Public.Api.Verenigingen.Search;

public class Metadata
{
    /// <summary>
    /// De paginatie metaData.
    /// </summary>
    public Pagination Pagination { get; init; }= null!;
}

public class Pagination
{
    /// <summary>
    /// het totaal aantal pagina's met de huidige limit.
    /// </summary>
    public long TotalCount { get; init; }
    /// <summary>
    /// Het aantal overgeslagen resultaten.
    /// </summary>
    public int Offset { get; init; }
    /// <summary>
    /// Het maximum aantal teruggegeven resultaten.
    /// </summary>
    public int Limit { get; init; }
}

public class PaginationQueryParams
{
    /// <summary>
    /// Het aantal items dat overgeslagen zal worden
    /// </summary>
    public int Offset { get; set; }

    /// <summary>
    /// Het aantal items dat (maximaal) zal worden opgehaald.
    /// </summary>
    /// <remarks>
    /// De laatste pagina kan minder items bevatten.
    /// </remarks>
    public int Limit { get; set; }
}
