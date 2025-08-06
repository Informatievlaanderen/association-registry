namespace AssociationRegistry.Admin.Api.Queries;

using DecentraalBeheer.Vereniging;
using Framework;
using Nest;
using Schema.Search;
using System.Text.RegularExpressions;
using Vereniging;
using WebApi.Verenigingen.Search;
using WebApi.Verenigingen.Search.RequestModels;

public interface IBeheerVerenigingenZoekQuery : IQuery<ISearchResponse<VerenigingZoekDocument>, BeheerVerenigingenZoekFilter>;

public class BeheerVerenigingenZoekQuery : IBeheerVerenigingenZoekQuery
{
    private readonly IElasticClient _client;
    private readonly ITypeMapping _typeMapping;

    private static readonly Func<SortDescriptor<VerenigingZoekDocument>, SortDescriptor<VerenigingZoekDocument>> DefaultSort =
        x => x.Descending(v => v.VCode);

    public BeheerVerenigingenZoekQuery(IElasticClient client, ITypeMapping typeMapping)
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
                .Query(query => MainQuery(filter, query)
                 )
                .TrackTotalHits(),
            cancellationToken
        );

    public static QueryContainer MainQuery(BeheerVerenigingenZoekFilter filter, QueryContainerDescriptor<VerenigingZoekDocument> query)
    {
        return query
           .Bool(boolQueryDescriptor => boolQueryDescriptor
                                       .Must(MatchWithQuery(filter.Query))
                                       .MustNot(BeVerwijderd(), BeDubbel())
            );
    }

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
