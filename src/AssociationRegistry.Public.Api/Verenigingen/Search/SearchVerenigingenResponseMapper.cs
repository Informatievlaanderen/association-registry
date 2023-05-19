namespace AssociationRegistry.Public.Api.Verenigingen.Search;

using System;
using System.Collections.Generic;
using System.Linq;
using Constants;
using Infrastructure.ConfigurationBindings;
using Nest;
using RequestModels;
using ResponseModels;
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
            Verenigingen = searchResponse.Hits
                .Select(x => Map(x.Source, _appSettings))
                .ToArray(),
            Facets = MapFacets(searchResponse, originalQuery, hoofdactiviteiten),
            Metadata = GetMetadata(searchResponse, paginationRequest),
        };

    private Facets MapFacets(ISearchResponse<VerenigingDocument> searchResponse, string originalQuery, string[] hoofdactiviteiten)
        => new()
        {
            HoofdactiviteitenVerenigingsloket = GetHoofdActiviteitFacets(_appSettings, searchResponse, originalQuery, hoofdactiviteiten),
        };

    private static Vereniging Map(VerenigingDocument verenigingDocument, AppSettings appSettings)
        => new()
        {
            VCode = verenigingDocument.VCode,
            Type = Map(verenigingDocument.Type),
            Naam = verenigingDocument.Naam,
            KorteNaam = verenigingDocument.KorteNaam,
            HoofdactiviteitenVerenigingsloket = verenigingDocument.HoofdactiviteitenVerenigingsloket
                .Select(Map)
                .ToArray(),
            Doelgroep = verenigingDocument.Doelgroep,
            Locaties = verenigingDocument.Locaties
                .Select(Map)
                .ToArray(),
            Activiteiten = verenigingDocument.Activiteiten
                .Select(Map)
                .ToArray(),
            Sleutels = verenigingDocument.Sleutels
                .Select(Map)
                .ToArray(),
            Links = Map(verenigingDocument.VCode, appSettings),
        };

    private static VerenigingLinks Map(string vCode, AppSettings appSettings)
        => new() { Detail = new Uri($"{appSettings.BaseUrl}/v1/verenigingen/{vCode}") };

    private static Activiteit Map(string activiteit)
        => new() { Id = -1, Categorie = activiteit };

    private static HoofdactiviteitVerenigingsloket Map(VerenigingDocument.HoofdactiviteitVerenigingsloket h)
        => new() { Code = h.Code, Beschrijving = h.Naam };

    private static VerenigingsType Map(VerenigingDocument.VerenigingsType verenigingDocumentType)
        => new()
        {
            Code = verenigingDocumentType.Code,
            Beschrijving = verenigingDocumentType.Beschrijving,
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

    private static Locatie Map(VerenigingDocument.Locatie loc)
        => new()
        {
            Locatietype = loc.Locatietype,
            Hoofdlocatie = loc.Hoofdlocatie,
            Adres = loc.Adres,
            Naam = loc.Naam,
            Postcode = loc.Postcode,
            Gemeente = loc.Gemeente,
        };


    private static Sleutel Map(VerenigingDocument.Sleutel s)
        => new()
        {
            Bron = s.Bron,
            Waarde = s.Waarde,
        };
}
