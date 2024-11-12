namespace AssociationRegistry.Admin.Api.Queries;

using Framework;
using Nest;
using Schema.Search;
using Verenigingen.Search;
using Verenigingen.Search.RequestModels;

public interface IBeheerVerenigingZoekQuery : IQuery<ISearchResponse<VerenigingZoekDocument>, BeheerVerenigingZoekFilter>;

public class BeheerVerenigingZoekQuery : IBeheerVerenigingZoekQuery
{
    private readonly IElasticClient _client;
    private readonly TypeMapping _typeMapping;

    private static readonly Func<SortDescriptor<VerenigingZoekDocument>, SortDescriptor<VerenigingZoekDocument>> DefaultSort =
        x => x.Descending(v => v.VCode);

    public BeheerVerenigingZoekQuery(IElasticClient client, TypeMapping typeMapping)
    {
        _client = client;
        _typeMapping = typeMapping;
    }

    private static Func<QueryContainerDescriptor<VerenigingZoekDocument>, QueryContainer> MatchWithQuery(string q)
    {
        return queryContainerDescriptor =>
            queryContainerDescriptor.QueryString(
                queryStringQueryDescriptor
                    => queryStringQueryDescriptor.Query(q)
            );
    }

    private static QueryContainer BeVerwijderd(QueryContainerDescriptor<VerenigingZoekDocument> shouldDescriptor)
    {
        return shouldDescriptor
           .Term(termDescriptor
                     => termDescriptor
                       .Field(document => document.IsVerwijderd)
                       .Value(true));
    }

    public async Task<ISearchResponse<VerenigingZoekDocument>> ExecuteAsync(BeheerVerenigingZoekFilter filter, CancellationToken cancellationToken)
        => await _client.SearchAsync<VerenigingZoekDocument>(
            s => s
                .From(filter.PaginationQueryParams.Offset)
                .Size(filter.PaginationQueryParams.Limit)
                .ParseSort(filter.Sort, DefaultSort, _typeMapping)
                .Query(query => query
                          .Bool(boolQueryDescriptor =>
                                    boolQueryDescriptor.Must(MatchWithQuery(filter.Query))
                                                       .MustNot(BeVerwijderd)
                           )
                 )
                .TrackTotalHits(),
            cancellationToken
        );
}

public record BeheerVerenigingZoekFilter
{
    public string Query { get; }
    public string? Sort { get; }
    public PaginationQueryParams PaginationQueryParams { get; }

    public BeheerVerenigingZoekFilter(string query, string? sort, PaginationQueryParams paginationQueryParams)
    {
        Query = query;
        Sort = sort;
        PaginationQueryParams = paginationQueryParams;
    }
}
