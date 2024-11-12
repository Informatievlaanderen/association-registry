namespace AssociationRegistry.Public.Api.Queries;

using Constants;
using Framework;
using Nest;
using Schema;
using Schema.Constants;
using Schema.Search;
using System.Text;
using Verenigingen.Search;
using Verenigingen.Search.RequestModels;

public interface IPubliekVerenigingenZoekQuery : IQuery<ISearchResponse<VerenigingZoekDocument>, PubliekVerenigingenZoekFilter>;

public class PubliekVerenigingenZoekQuery : IPubliekVerenigingenZoekQuery
{
    private readonly IElasticClient _client;
    private readonly TypeMapping _typeMapping;

    private static readonly Func<SortDescriptor<VerenigingZoekDocument>, SortDescriptor<VerenigingZoekDocument>> DefaultSort =
        x => x.Descending(v => v.VCode);

    public PubliekVerenigingenZoekQuery(IElasticClient client, TypeMapping typeMapping)
    {
        _client = client;
        _typeMapping = typeMapping;
    }

    public async Task<ISearchResponse<VerenigingZoekDocument>> ExecuteAsync(PubliekVerenigingenZoekFilter filter, CancellationToken cancellationToken)
        => await _client.SearchAsync<VerenigingZoekDocument>(
            s =>
            {
                return s
                      .From(filter.PaginationQueryParams.Offset)
                      .Size(filter.PaginationQueryParams.Limit)
                      .ParseSort(filter.Sort, DefaultSort, _typeMapping)
                      .Query(query => query
                                .Bool(boolQueryDescriptor => boolQueryDescriptor
                                                            .Must(queryContainerDescriptor
                                                                      => MatchQueryString(
                                                                          queryContainerDescriptor,
                                                                          $"{filter.Query}{BuildHoofdActiviteiten(filter.Hoofdactiviteiten)}"),
                                                                  BeActief
                                                             )
                                                            .MustNot(
                                                                 BeUitgeschrevenUitPubliekeDatastroom,
                                                                 BeRemoved)
                                 )
                       )
                      .Aggregations(
                           agg =>
                               GlobalAggregation(
                                   agg,
                                   aggregations: agg2 =>
                                       QueryFilterAggregation(
                                           agg2,
                                           filter.Query,
                                           HoofdactiviteitCountAggregation
                                       )
                               )
                       )
                      .TrackTotalHits();
            }, cancellationToken);

    private async Task<ISearchResponse<VerenigingZoekDocument>> Search(
        string q,
        string? sort,
        string[] hoofdactiviteiten,
        PaginationQueryParams paginationQueryParams)
        => await _client.SearchAsync<VerenigingZoekDocument>(
            s =>
            {
                return s
                      .From(paginationQueryParams.Offset)
                      .Size(paginationQueryParams.Limit)
                      .ParseSort(sort, DefaultSort, _typeMapping)
                      .Query(query => query
                                .Bool(boolQueryDescriptor => boolQueryDescriptor
                                                            .Must(queryContainerDescriptor
                                                                      => MatchQueryString(
                                                                          queryContainerDescriptor,
                                                                          $"{q}{BuildHoofdActiviteiten(hoofdactiviteiten)}"),
                                                                  BeActief
                                                             )
                                                            .MustNot(
                                                                 BeUitgeschrevenUitPubliekeDatastroom,
                                                                 BeRemoved)
                                 )
                       )
                      .Aggregations(
                           agg =>
                               GlobalAggregation(
                                   agg,
                                   aggregations: agg2 =>
                                       QueryFilterAggregation(
                                           agg2,
                                           q,
                                           HoofdactiviteitCountAggregation
                                       )
                               )
                       );
            });

    private static IAggregationContainer GlobalAggregation<T>(
        AggregationContainerDescriptor<T> agg,
        Func<AggregationContainerDescriptor<T>, AggregationContainerDescriptor<T>> aggregations) where T : class
    {
        agg.Global(
            WellknownFacets.GlobalAggregateName,
            selector: d => d.Aggregations(
                aggregations
            )
        );

        return agg;
    }

    private static AggregationContainerDescriptor<T> QueryFilterAggregation<T>(
        AggregationContainerDescriptor<T> aggregationContainerDescriptor,
        string query,
        Func<AggregationContainerDescriptor<T>, IAggregationContainer> aggregations)
        where T : class, ICanBeUitgeschrevenUitPubliekeDatastroom, IHasStatus, IDeletable
    {
        return aggregationContainerDescriptor.Filter(
            WellknownFacets.FilterAggregateName,
            selector: aggregationDescriptor =>
                aggregationDescriptor
                   .Filter(containerDescriptor =>
                               containerDescriptor
                                  .Bool(queryDescriptor =>
                                            queryDescriptor
                                               .Must(m =>
                                                         MatchQueryString(m, query),
                                                     BeActief
                                                )
                                               .MustNot(
                                                    BeUitgeschrevenUitPubliekeDatastroom,
                                                    BeRemoved)
                                   )
                    )
                   .Aggregations(aggregations)
        );
    }

    private static QueryContainer MatchQueryString<T>(QueryContainerDescriptor<T> m, string query)
        where T : class, ICanBeUitgeschrevenUitPubliekeDatastroom
    {
        return m.QueryString(
            qs =>
                qs.Query(query)
                  .Analyzer(VerenigingZoekDocumentMapping.PubliekZoekenAnalyzer)
        );
    }

    private static AggregationContainerDescriptor<VerenigingZoekDocument> HoofdactiviteitCountAggregation(
        AggregationContainerDescriptor<VerenigingZoekDocument> aggregationContainerDescriptor)
    {
        return aggregationContainerDescriptor.Terms(
            WellknownFacets.HoofdactiviteitenCountAggregateName,
            selector: valueCountAggregationDescriptor => valueCountAggregationDescriptor
                                                        .Field(document => document.HoofdactiviteitenVerenigingsloket.Select(
                                                                   h => h.Code).Suffix("keyword")
                                                         )
                                                        .Size(size: AssociationRegistry.Vereniging.HoofdactiviteitVerenigingsloket
                                                                                       .HoofdactiviteitenVerenigingsloketCount)
        );
    }

    private static string BuildHoofdActiviteiten(IReadOnlyCollection<string> hoofdactiviteiten)
    {
        if (hoofdactiviteiten.Count == 0)
            return string.Empty;

        var builder = new StringBuilder();
        builder.Append(" AND (");

        foreach (var (hoofdactiviteit, index) in hoofdactiviteiten.Select((item, index) => (item, index)))
        {
            builder.Append($"hoofdactiviteitenVerenigingsloket.code:{hoofdactiviteit}");

            if (index < hoofdactiviteiten.Count - 1)
                builder.Append(" OR ");
        }

        builder.Append(value: ')');

        return builder.ToString();
    }

    private static QueryContainer BeUitgeschrevenUitPubliekeDatastroom<T>(QueryContainerDescriptor<T> q)
        where T : class, ICanBeUitgeschrevenUitPubliekeDatastroom
    {
        return q.Term(field: arg => arg.IsUitgeschrevenUitPubliekeDatastroom, value: true);
    }

    private static QueryContainer BeActief<T>(QueryContainerDescriptor<T> q)
        where T : class, IHasStatus
    {
        return q.Term(field: arg => arg.Status, VerenigingStatus.Actief);
    }

    private static QueryContainer BeRemoved<T>(QueryContainerDescriptor<T> q)
        where T : class, IDeletable
    {
        return q.Term(field: arg => arg.IsVerwijderd, value: true);
    }
}

public record PubliekVerenigingenZoekFilter
{
    public string Query { get; }
    public string? Sort { get; }
    public string[] Hoofdactiviteiten { get; }
    public PaginationQueryParams PaginationQueryParams { get; }

    public PubliekVerenigingenZoekFilter(string query, string? sort, string[] hoofdactiviteiten, PaginationQueryParams paginationQueryParams)
    {
        Query = query;
        Sort = sort;
        Hoofdactiviteiten = hoofdactiviteiten;
        PaginationQueryParams = paginationQueryParams;
    }
}
