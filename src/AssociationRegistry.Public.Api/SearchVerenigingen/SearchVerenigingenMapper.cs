namespace AssociationRegistry.Public.Api.SearchVerenigingen;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Constants;
using Nest;

public class SearchVerenigingenMapper
{
    private readonly AppSettings _appSettings;

    public SearchVerenigingenMapper(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public SearchVerenigingenResponse ToSearchVereningenResponse(
        ISearchResponse<VerenigingDocument> searchResponse,
        PaginationQueryParams paginationRequest,
        string originalQuery)
        => new()
        {
            Verenigingen = GetVerenigingenFromResponse(_appSettings, searchResponse),
            Facets = new Facets
            {
                HoofdActiviteiten = GetHoofdActiviteitFacets(_appSettings, searchResponse, originalQuery),
            },
            Metadata = GetMetadata(searchResponse, paginationRequest),
        };

    private static Metadata GetMetadata(ISearchResponse<VerenigingDocument> searchResponse, PaginationQueryParams paginationRequest)
        => new(
            new Pagination(
                searchResponse.Total,
                paginationRequest.Offset,
                paginationRequest.Limit
            )
        );

    private static ImmutableArray<HoofdActiviteitFacetItem> GetHoofdActiviteitFacets(AppSettings appSettings, ISearchResponse<VerenigingDocument> searchResponse, string originalQuery)
        => searchResponse.Aggregations
            .Terms(WellknownFacets.HoofdactiviteitenCountAggregateName)
            .Buckets
            .Select(bucket => CreateHoofdActiviteitFacetItem(appSettings, bucket, originalQuery))
            .ToImmutableArray();

    private static HoofdActiviteitFacetItem CreateHoofdActiviteitFacetItem(AppSettings appSettings, KeyedBucket<string> bucket, string originalQuery)
        => CreateHoofdActiviteitFacetItem(appSettings, Activiteiten.Hoofdactiviteit.Create(bucket.Key), bucket.DocCount ?? 0, originalQuery);

    private static HoofdActiviteitFacetItem CreateHoofdActiviteitFacetItem(AppSettings appSettings, Activiteiten.Hoofdactiviteit hoofdActiviteit, long aantal, string originalQuery)
        => new(hoofdActiviteit.Code, hoofdActiviteit.Naam, aantal, AddHoofdactiviteitToQuery(appSettings, hoofdActiviteit.Code, originalQuery));

    // public for testing
    public static string AddHoofdactiviteitToQuery(AppSettings appSettings, string hoofdActiviteitCode, string originalQuery)
        => $"{appSettings.BaseUrl}v1/verenigingen/zoeken?q={CalculateQuery(originalQuery, hoofdActiviteitCode)}";

    private static string CalculateQuery(string? originalQuery, string hoofdActiviteitCode)
    {
        var originalQueryContainsHoofdactiviteitenFacets = originalQuery?.Contains("hoofdactiviteiten.code") ?? false;
        if (!originalQueryContainsHoofdactiviteitenFacets)
            return string.IsNullOrWhiteSpace(originalQuery) || originalQuery.Trim() == "*"
                ? $"(hoofdactiviteiten.code:{hoofdActiviteitCode})"
                : $"(hoofdactiviteiten.code:{hoofdActiviteitCode}) AND {originalQuery}";

        var regex = new Regex(@"\((hoofdactiviteiten\.code:[A-Z]{4}( OR hoofdactiviteiten\.code:[A-Z]{4})*)\)( AND .+)?");
        var match = regex.Match(originalQuery ?? string.Empty);

        return $"({match.Groups[1].Value} OR hoofdactiviteiten.code:{hoofdActiviteitCode}){match.Groups[3].Value}";
    }

    private static ImmutableArray<Vereniging> GetVerenigingenFromResponse(AppSettings appSettings, ISearchResponse<VerenigingDocument> searchResponse)
        => searchResponse.Hits.Select(
            x =>
            {
                return new Vereniging(
                    x.Source.VCode,
                    x.Source.Naam,
                    x.Source.KorteNaam ?? string.Empty,
                    x.Source.Hoofdactiviteiten.Select(h => new Hoofdactiviteit(h.Code, h.Naam)).ToImmutableArray(),
                    x.Source.Doelgroep,
                    x.Source.Locaties.Select(ToLocatieResponse).ToImmutableArray(),
                    x.Source.Activiteiten.Select(activiteit => new Activiteit(-1, activiteit)).ToImmutableArray(),
                    new VerenigingLinks(new Uri($"{appSettings.BaseUrl}v1/verenigingen/{(string?)x.Source.VCode}"))
                );
            }).ToImmutableArray();

    private static Locatie ToLocatieResponse(VerenigingDocument.Locatie loc)
        => new(loc.Type, loc.Hoofdlocatie, loc.Adres, loc.Naam, loc.Postcode, loc.Gemeente);
}
