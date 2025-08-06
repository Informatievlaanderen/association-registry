namespace AssociationRegistry.Admin.Api.Queries;

using Framework;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Mapping;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Hosts.Configuration.ConfigurationBindings;
using Schema.Search;
using System.Text.RegularExpressions;
using Vereniging;
using WebApi.Verenigingen.Search;
using WebApi.Verenigingen.Search.RequestModels;

public interface IBeheerVerenigingenZoekQuery : IQuery<SearchResponse<VerenigingZoekDocument>, BeheerVerenigingenZoekFilter>;

public class BeheerVerenigingenZoekQuery : IBeheerVerenigingenZoekQuery
{
    private readonly ElasticsearchClient _client;
    private readonly TypeMapping _typeMapping;
    private readonly ElasticSearchOptionsSection _elasticSearchOptionsSection;

    public BeheerVerenigingenZoekQuery(ElasticsearchClient client, TypeMapping typeMapping, ElasticSearchOptionsSection elasticSearchOptionsSection)
    {
        _client = client;
        _typeMapping = typeMapping;
        _elasticSearchOptionsSection = elasticSearchOptionsSection;
    }

    public async Task<SearchResponse<VerenigingZoekDocument>> ExecuteAsync(BeheerVerenigingenZoekFilter filter, CancellationToken cancellationToken)
    {
        var sort = SearchVerenigingenExtensions.ParseSort(
            filter.Sort,
            new List<SortOptions> {
                new()
                {
                    Field = new FieldSort {
                        Field = FieldTerms.VCode,
                        Order = SortOrder.Desc,
                    },
                },
            },
            _typeMapping);

        var searchRequest = new SearchRequest<VerenigingZoekDocument>(_elasticSearchOptionsSection.Indices!.Verenigingen!)
        {
            From = filter.PaginationQueryParams.Offset,
            Size = filter.PaginationQueryParams.Limit,
            Sort = sort,
            Query = BuildQuery(filter),
            TrackTotalHits = true,
        };

        return await _client.SearchAsync<VerenigingZoekDocument>(searchRequest, cancellationToken);
    }

    private static Query BuildQuery(BeheerVerenigingenZoekFilter filter)
        => new BoolQuery
        {
            Must = new List<Query>
            {
                new QueryStringQuery
                {
                    Query = filter.Query,
                }
            },
            MustNot = new List<Query>
            {
                new TermQuery
                {
                    Field = FieldTerms.IsVerwijderd,
                    Value = true,
                },
                new TermQuery
                {
                    Field = FieldTerms.IsDubbel,
                    Value = true,
                }
            }
        };
}


public record BeheerVerenigingenZoekFilter
{
    public static readonly string ExpandedVerenigingsType = $"(verenigingstype.code:{Verenigingstype.VZER.Code} OR verenigingstype.code:{Verenigingstype.FeitelijkeVereniging.Code})";
    public string Query { get; }
    public string? Sort { get; }
    public PaginationQueryParams PaginationQueryParams { get; }

    public BeheerVerenigingenZoekFilter(string query, string? sort, PaginationQueryParams paginationQueryParams)
    {
        Query = ExpandVerenigingsTypeForVzerMigration(query);
        Sort = sort;
        PaginationQueryParams = paginationQueryParams;
    }

    static BeheerVerenigingenZoekFilter()
    {
    }

    private static string ExpandVerenigingsTypeForVzerMigration(string query)
    {
        var replacement = ExpandedVerenigingsType;

        var pattern = $@"\bverenigingstype.code\s*:\s*({Verenigingstype.VZER.Code}|{Verenigingstype.FeitelijkeVereniging.Code})\b"; // Capture any value after type:

        return Regex.Replace(query, pattern, replacement, RegexOptions.IgnoreCase);
    }
}
