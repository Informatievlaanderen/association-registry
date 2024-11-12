namespace AssociationRegistry.Admin.Api.Verenigingen.Search;

using Nest;
using RequestModels;
using Schema.Search;

public static class VerenigingZoekDocumentQuery
{
    private static readonly Func<SortDescriptor<VerenigingZoekDocument>, SortDescriptor<VerenigingZoekDocument>> DefaultSort =
        x => x.Descending(v => v.VCode);

    public static async Task<ISearchResponse<VerenigingZoekDocument>> Search(
        IElasticClient client,
        string q,
        string? sort,
        PaginationQueryParams paginationQueryParams,
        TypeMapping typemapping)
        => await client.SearchAsync<VerenigingZoekDocument>(
            s => s
                .From(paginationQueryParams.Offset)
                .Size(paginationQueryParams.Limit)
                .ParseSort(sort, DefaultSort, typemapping)
                .Query(query => query
                          .Bool(boolQueryDescriptor =>
                                    boolQueryDescriptor.Must(MatchWithQuery(q))
                                                       .MustNot(BeVerwijderd)
                           )
                 )
                .TrackTotalHits()
        );

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
}
