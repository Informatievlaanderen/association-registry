namespace AssociationRegistry.Admin.Api.Queries;

using DecentraalBeheer.Verenigingen.Search;
using DecentraalBeheer.Verenigingen.Search.RequestModels;
using Framework;
using Nest;
using Schema.Search;

public interface IBeheerVerenigingenZoekQuery : IQuery<ISearchResponse<VerenigingZoekDocument>, BeheerVerenigingenZoekFilter>;

public class BeheerVerenigingenZoekQuery : IBeheerVerenigingenZoekQuery
{
    private readonly IElasticClient _client;
    private readonly TypeMapping _typeMapping;

    private static readonly Func<SortDescriptor<VerenigingZoekDocument>, SortDescriptor<VerenigingZoekDocument>> DefaultSort =
        x => x.Descending(v => v.VCode);

    public BeheerVerenigingenZoekQuery(IElasticClient client, TypeMapping typeMapping)
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

    public async Task<ISearchResponse<VerenigingZoekDocument>> ExecuteAsync(BeheerVerenigingenZoekFilter filter, CancellationToken cancellationToken)
        => await _client.SearchAsync<VerenigingZoekDocument>(
            s => s
                .From(filter.PaginationQueryParams.Offset)
                .Size(filter.PaginationQueryParams.Limit)
                .ParseSort(filter.Sort, DefaultSort, _typeMapping)
                .Query(query => query
                          .Bool(boolQueryDescriptor => boolQueryDescriptor
                                                      .Must(MatchWithQuery(filter.Query))
                                                      .MustNot(BeVerwijderd(), BeDubbel())
                           )
                 )
                .TrackTotalHits(),
            cancellationToken
        );

    private static Func<QueryContainerDescriptor<VerenigingZoekDocument>, QueryContainer> BeVerwijderd()
    {
        return q => q
           .Term(t => t
                     .Field(f => f.IsVerwijderd)
                     .Value(true));
    }

    private static Func<QueryContainerDescriptor<VerenigingZoekDocument>, QueryContainer> BeDubbel()
    {
        return q => q
           .Term(t => t
                     .Field(f => f.IsDubbel)
                     .Value(true));
    }
}

public record BeheerVerenigingenZoekFilter
{
    public string Query { get; }
    public string? Sort { get; }
    public PaginationQueryParams PaginationQueryParams { get; }

    public BeheerVerenigingenZoekFilter(string query, string? sort, PaginationQueryParams paginationQueryParams)
    {
        Query = query;
        Sort = sort;
        PaginationQueryParams = paginationQueryParams;
    }
}
