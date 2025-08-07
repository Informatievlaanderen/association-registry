namespace AssociationRegistry.Public.Api.Queries;

using Constants;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Aggregations;
using Elastic.Clients.Elasticsearch.Fluent;
using Elastic.Clients.Elasticsearch.Mapping;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Framework;
using Hosts.Configuration.ConfigurationBindings;
using Schema;
using Schema.Search;
using System.Text;
using System.Text.RegularExpressions;
using Vereniging;
using Verenigingen.Search;
using Verenigingen.Search.RequestModels;
using static Elastic.Clients.Elasticsearch.QueryDsl.Query;

public interface IPubliekVerenigingenZoekQuery : IQuery<SearchResponse<VerenigingZoekDocument>, PubliekVerenigingenZoekFilter>;

public class PubliekVerenigingenZoekQuery : IPubliekVerenigingenZoekQuery
{
    private readonly ElasticsearchClient _client;
    private readonly TypeMapping _typeMapping;
    private readonly ElasticSearchOptionsSection _elasticSearchOptionsSection;

    public PubliekVerenigingenZoekQuery(ElasticsearchClient client, TypeMapping typeMapping, ElasticSearchOptionsSection elasticSearchOptionsSection)
    {
        _client = client;
        _typeMapping = typeMapping;
        _elasticSearchOptionsSection = elasticSearchOptionsSection;
    }

    public async Task<SearchResponse<VerenigingZoekDocument>> ExecuteAsync(
        PubliekVerenigingenZoekFilter filter,
        CancellationToken cancellationToken)
    {
        var response = await _client.SearchAsync<VerenigingZoekDocument>(s =>
                                                                             s
                                                                                .Indices(_elasticSearchOptionsSection.Indices.Verenigingen)
                                                                                .From(filter.PaginationQueryParams.Offset)
                                                                                .Size(filter.PaginationQueryParams.Limit)
                                                                                .Query(MainQuery(filter))
                                                                                .Sort(ParseSort(filter.Sort))
                                                                                .Aggregations(Aggregation(filter))
                                                                                .TrackTotalHits(true),
                                                                         cancellationToken);

        return response;
    }

    private static Action<FluentDictionaryOfStringAggregation<VerenigingZoekDocument>>? Aggregation(PubliekVerenigingenZoekFilter filter)
    {
        return agg => agg
           .Add(WellknownFacets.GlobalAggregateName, global => global
                                                              .Global()
                                                              .Aggregations(agg2 => agg2
                                                                               .Add(WellknownFacets.FilterAggregateName, f => f
                                                                                       .Filter(f => f
                                                                                           .Bool(b => b
                                                                                               .Must(
                                                                                                    MatchQueryString(
                                                                                                        filter.Query),
                                                                                                    BeActief()
                                                                                                )
                                                                                               .MustNot(
                                                                                                    BeUitgeschrevenUitPubliekeDatastroom(),
                                                                                                    BeRemoved(),
                                                                                                    BeDubbel()
                                                                                                )
                                                                                            )
                                                                                        )
                                                                                       .Aggregations(agg3 => agg3
                                                                                           .Add(
                                                                                                WellknownFacets
                                                                                                   .HoofdactiviteitenCountAggregateName,
                                                                                                terms => terms
                                                                                                   .Terms(t => t
                                                                                                       .Field(
                                                                                                            "hoofdactiviteitenVerenigingsloket.code.keyword")
                                                                                                       .Size(
                                                                                                            AssociationRegistry
                                                                                                               .Vereniging
                                                                                                               .HoofdactiviteitVerenigingsloket
                                                                                                               .HoofdactiviteitenVerenigingsloketCount)
                                                                                                    )
                                                                                            )
                                                                                        )
                                                                                )
                                                               )
            );
    }

    public static Query MainQuery(PubliekVerenigingenZoekFilter filter)
    {
        return new BoolQuery
        {
            Must = new List<Query>
            {
                MatchQueryString($"{filter.Query}{BuildHoofdActiviteiten(filter.Hoofdactiviteiten)}"),
                BeActief()
            },
            MustNot = new List<Query>
            {
                BeUitgeschrevenUitPubliekeDatastroom(),
                BeRemoved(),
                BeDubbel()
            }
        };
    }

    private static AggregationDescriptor<VerenigingZoekDocument> BuildPubliekFacetsAggregation(PubliekVerenigingenZoekFilter filter)
    {
        return new AggregationDescriptor<VerenigingZoekDocument>()
           .AddAggregation(WellknownFacets.GlobalAggregateName, global => global
                                                                         .Global()
                                                                         .Aggregations(agg2 => agg2
                                                                                          .Add(WellknownFacets.FilterAggregateName, f => f
                                                                                              .Filter(fq => fq
                                                                                                  .Bool(b => b
                                                                                                      .Must(
                                                                                                           MatchQueryString(
                                                                                                               filter
                                                                                                                  .Query),
                                                                                                           BeActief()
                                                                                                       )
                                                                                                      .MustNot(
                                                                                                           BeUitgeschrevenUitPubliekeDatastroom(),
                                                                                                           BeRemoved(),
                                                                                                           BeDubbel()
                                                                                                       )
                                                                                                   )
                                                                                               )
                                                                                              .Aggregations(agg3 => agg3
                                                                                                  .Add(
                                                                                                       WellknownFacets
                                                                                                          .HoofdactiviteitenCountAggregateName,
                                                                                                       terms => terms
                                                                                                          .Terms(t => t
                                                                                                              .Field(
                                                                                                                   "hoofdactiviteitenVerenigingsloket.code.keyword")
                                                                                                              .Size(
                                                                                                                   AssociationRegistry
                                                                                                                      .Vereniging
                                                                                                                      .HoofdactiviteitVerenigingsloket
                                                                                                                      .HoofdactiviteitenVerenigingsloketCount)
                                                                                                           )
                                                                                                   )
                                                                                               )
                                                                                           )
                                                                          )
            );
    }

    private static Query MatchQueryString(string query)
    {
        return new QueryStringQuery
        {
            Query = query,
            Analyzer = VerenigingZoekDocumentMapping.PubliekZoekenAnalyzer
        };
    }

    private List<SortOptions> ParseSort(string? sortFilter)
    {
        var sort = SearchVerenigingenExtensions.ParseSort(
            sortFilter,
            new List<SortOptions> {
                new SortOptions {
                    Field = new FieldSort {
                        Field = "vCode",
                        Order = SortOrder.Desc,
                    }
                }
            },
            _typeMapping);

        return sort;
    }

    private static string BuildHoofdActiviteiten(IReadOnlyCollection<string> hoofdactiviteiten)
    {
        if (hoofdactiviteiten.Count == 0)
            return string.Empty;

        var builder = new StringBuilder(" AND (");

        for (int i = 0; i < hoofdactiviteiten.Count; i++)
        {
            builder.Append($"hoofdactiviteitenVerenigingsloket.code:{hoofdactiviteiten.ElementAt(i)}");

            if (i < hoofdactiviteiten.Count - 1)
                builder.Append(" OR ");
        }

        builder.Append(')');

        return builder.ToString();
    }

    private static Query BeUitgeschrevenUitPubliekeDatastroom()
        => TermQuery("isUitgeschrevenUitPubliekeDatastroom", true);

    private static Query BeRemoved()
        => TermQuery("isVerwijderd", true);

    private static Query BeDubbel()
        => TermQuery("isDubbel", true);

    private static Query BeActief()
        => TermQuery("status", Schema.Constants.VerenigingStatus.Actief);


    private static TermQuery TermQuery<T>(string field, T value) where T : notnull
        => new TermQuery
        {
            Field = field,
            Value = FieldValue.FromValue(value)
        };
}

public record PubliekVerenigingenZoekFilter
{
    public string Query { get; }
    public string? Sort { get; }
    public string[] Hoofdactiviteiten { get; }
    public PaginationQueryParams PaginationQueryParams { get; }

    public PubliekVerenigingenZoekFilter(
        string query,
        string? sort,
        string[] hoofdactiviteiten,
        PaginationQueryParams paginationQueryParams)
    {
        Query = ReplaceVerenigingstype(query);
        Sort = sort;
        Hoofdactiviteiten = hoofdactiviteiten;
        PaginationQueryParams = paginationQueryParams;
    }

    private string ReplaceVerenigingstype(string query)
    {
        var replacement =
            $"(verenigingstype.code:{Verenigingstype.VZER.Code} OR verenigingstype.code:{Verenigingstype.FeitelijkeVereniging.Code})";

        var pattern = $@"\bverenigingstype.code\s*:\s*({Verenigingstype.VZER.Code}|{Verenigingstype.FeitelijkeVereniging.Code})\b";

        return Regex.Replace(query, pattern, replacement, RegexOptions.IgnoreCase);
    }
}
