namespace AssociationRegistry.Public.Api.Verenigingen.Search;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Constants;
using Infrastructure.ConfigurationBindings;
using AssociationRegistry.Public.Schema.Search;
using Nest;

public class SearchVerenigingenResponseMapper
{
    private readonly AppSettings _appSettings;

    public SearchVerenigingenResponseMapper(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public SearchVerenigingenResponse ToSearchVereningenResponse(
        ISearchResponse<VerenigingDocument> searchResponse,
        PaginationQueryParams paginationRequest,
        string originalQuery,
        string[] hoofdactiviteiten)
        => new()
        {
            Verenigingen = GetVerenigingenFromResponse(_appSettings, searchResponse),
            Facets = new Facets
            {
                HoofdActiviteiten = GetHoofdActiviteitFacets(_appSettings, searchResponse, originalQuery, hoofdactiviteiten),
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

    private static ImmutableArray<HoofdActiviteitFacetItem> GetHoofdActiviteitFacets(
        AppSettings appSettings,
        ISearchResponse<VerenigingDocument> searchResponse,
        string originalQuery,
        string[] hoofdactiviteiten)
    {
        return searchResponse.Aggregations
            .Filter(WellknownFacets.GlobalAggregateName)
            .Filter(WellknownFacets.FilterAggregateName)
            .Terms(WellknownFacets.HoofdactiviteitenCountAggregateName)
            .Buckets
            .Select(bucket => CreateHoofdActiviteitFacetItem(appSettings, bucket, originalQuery, hoofdactiviteiten))
            .ToImmutableArray();
    }

    private static HoofdActiviteitFacetItem CreateHoofdActiviteitFacetItem(
        AppSettings appSettings,
        KeyedBucket<string> bucket,
        string originalQuery,
        string[] originalHoofdactiviteiten)
        => CreateHoofdActiviteitFacetItem(
            appSettings,
            Activiteiten.Hoofdactiviteit.Create(bucket.Key),
            bucket.DocCount ?? 0,
            originalQuery,
            originalHoofdactiviteiten);

    private static HoofdActiviteitFacetItem CreateHoofdActiviteitFacetItem(
        AppSettings appSettings,
        Activiteiten.Hoofdactiviteit facetHoofdActiviteit,
        long aantal,
        string originalQuery,
        string[] originalHoofdactiviteiten)
        => new(
            facetHoofdActiviteit.Code,
            facetHoofdActiviteit.Naam,
            aantal,
            AddHoofdactiviteitToQuery(
                appSettings,
                facetHoofdActiviteit.Code,
                originalQuery,
                originalHoofdactiviteiten));

    // public for testing
    public static string AddHoofdactiviteitToQuery(AppSettings appSettings, string hoofdActiviteitCode, string originalQuery, string[] hoofdactiviteiten)
        => $"{appSettings.BaseUrl}/v1/verenigingen/zoeken?q={originalQuery}&facets.hoofdactiviteiten={CalculateHoofdactiviteiten(hoofdactiviteiten, hoofdActiviteitCode)}";

    private static string CalculateHoofdactiviteiten(IEnumerable<string> originalHoofdactiviteiten, string hoofdActiviteitCode)
        => string.Join(
            ',',
            originalHoofdactiviteiten.Append(hoofdActiviteitCode).Select(x => x.ToUpperInvariant()).Distinct());

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
                    new VerenigingLinks(new Uri($"{appSettings.BaseUrl}/v1/verenigingen/{(string?)x.Source.VCode}"))
                );
            }).ToImmutableArray();

    private static Locatie ToLocatieResponse(VerenigingDocument.Locatie loc)
        => new(loc.Locatietype, loc.Hoofdlocatie, loc.Adres, loc.Naam, loc.Postcode, loc.Gemeente);
}
