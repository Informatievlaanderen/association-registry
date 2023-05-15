namespace AssociationRegistry.Public.Api.Verenigingen.Search;

using System;
using System.Collections.Generic;
using System.Linq;
using Constants;
using Infrastructure.ConfigurationBindings;
using Nest;
using Schema.Search;

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
                HoofdactiviteitenVerenigingsloket = GetHoofdActiviteitFacets(_appSettings, searchResponse, originalQuery, hoofdactiviteiten),
            },
            Metadata = GetMetadata(searchResponse, paginationRequest),
        };

    private static Metadata GetMetadata(ISearchResponse<VerenigingDocument> searchResponse, PaginationQueryParams paginationRequest)
        => new()
        {
            Pagination = new Pagination
            {
                TotalCount = searchResponse.Total,
                Offset = paginationRequest.Offset,
                Limit = paginationRequest.Limit,
            },
        };

    private static HoofdactiviteitVerenigingsloketFacetItem[] GetHoofdActiviteitFacets(
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
            .ToArray();
    }

    private static HoofdactiviteitVerenigingsloketFacetItem CreateHoofdActiviteitFacetItem(
        AppSettings appSettings,
        KeyedBucket<string> bucket,
        string originalQuery,
        string[] originalHoofdactiviteiten)
        => new()
        {
            Code = bucket.Key,
            Aantal = bucket.DocCount ?? 0,
            Query = AddHoofdactiviteitToQuery(
                appSettings,
                bucket.Key,
                originalQuery,
                originalHoofdactiviteiten),
        };

    // public for testing
    public static string AddHoofdactiviteitToQuery(AppSettings appSettings, string hoofdactiviteitenVerenigingsloketCode, string originalQuery, string[] hoofdactiviteiten)
        => $"{appSettings.BaseUrl}/v1/verenigingen/zoeken?q={originalQuery}&facets.hoofdactiviteitenVerenigingsloket={CalculateHoofdactiviteiten(hoofdactiviteiten, hoofdactiviteitenVerenigingsloketCode)}";

    private static string CalculateHoofdactiviteiten(IEnumerable<string> originalHoofdactiviteiten, string hoofdActiviteitCode)
        => string.Join(
            separator: ',',
            originalHoofdactiviteiten.Append(hoofdActiviteitCode).Select(x => x.ToUpperInvariant()).Distinct());

    private static Vereniging[] GetVerenigingenFromResponse(AppSettings appSettings, ISearchResponse<VerenigingDocument> searchResponse)
        => searchResponse.Hits.Select(
            x =>
            {
                return new Vereniging()
                {
                    VCode = x.Source.VCode,
                    Type = new VerenigingsType()
                    {
                        Code = x.Source.Type.Code,
                        Beschrijving = x.Source.Type.Beschrijving,
                    },
                    Naam = x.Source.Naam,
                    KorteNaam = x.Source.KorteNaam,
                    HoofdactiviteitenVerenigingsloket = x.Source.HoofdactiviteitenVerenigingsloket
                        .Select(h => new HoofdactiviteitVerenigingsloket() { Code = h.Code, Beschrijving = h.Naam })
                        .ToArray(),
                    Doelgroep = x.Source.Doelgroep,
                    Locaties = x.Source.Locaties
                        .Select(ToLocatieResponse)
                        .ToArray(),
                    Activiteiten = x.Source.Activiteiten
                        .Select(activiteit => new Activiteit() { Id = -1, Categorie = activiteit })
                        .ToArray(),
                    Links = new VerenigingLinks() { Detail = new Uri($"{appSettings.BaseUrl}/v1/verenigingen/{(string?)x.Source.VCode}") },
                };
            }).ToArray();

    private static Locatie ToLocatieResponse(VerenigingDocument.Locatie loc)
        => new()
        {
            Locatietype = loc.Locatietype,
            Hoofdlocatie = loc.Hoofdlocatie,
            Adres = loc.Adres,
            Naam = loc.Naam,
            Postcode = loc.Postcode,
            Gemeente = loc.Gemeente,
        };
}
