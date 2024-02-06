namespace AssociationRegistry.Public.Api.Verenigingen.Search;

using Constants;
using Infrastructure.ConfigurationBindings;
using Nest;
using RequestModels;
using ResponseModels;
using Schema.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using Relatie = ResponseModels.Relatie;

public class SearchVerenigingenResponseMapper
{
    private readonly AppSettings _appSettings;

    public SearchVerenigingenResponseMapper(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public SearchVerenigingenResponse ToSearchVereningenResponse(
        ISearchResponse<VerenigingZoekDocument> searchResponse,
        PaginationQueryParams paginationRequest,
        string originalQuery,
        string[] hoofdactiviteiten)
        => new()
        {
            Context = $"{_appSettings.BaseUrl}/v1/contexten/publiek/zoek-verenigingen-context.json",
            Verenigingen = searchResponse.Hits
                                         .Select(x => Map(x.Source, _appSettings))
                                         .ToArray(),
            Facets = MapFacets(searchResponse, originalQuery, hoofdactiviteiten),
            Metadata = GetMetadata(searchResponse, paginationRequest),
        };

    private Facets MapFacets(ISearchResponse<VerenigingZoekDocument> searchResponse, string originalQuery, string[] hoofdactiviteiten)
        => new()
        {
            HoofdactiviteitenVerenigingsloket = GetHoofdActiviteitFacets(_appSettings, searchResponse, originalQuery, hoofdactiviteiten),
        };

    private static Vereniging Map(VerenigingZoekDocument verenigingZoekDocument, AppSettings appSettings)
        => new()
        {
            id = verenigingZoekDocument.JsonLdMetadata.Id,
            type = verenigingZoekDocument.JsonLdMetadata.Type,
            VCode = verenigingZoekDocument.VCode,
            Verenigingstype = Map(verenigingZoekDocument.Verenigingstype),
            Naam = verenigingZoekDocument.Naam,
            Roepnaam = verenigingZoekDocument.Roepnaam,
            KorteNaam = verenigingZoekDocument.KorteNaam,
            KorteBeschrijving = verenigingZoekDocument.KorteBeschrijving,
            Doelgroep = Map(verenigingZoekDocument.Doelgroep),
            HoofdactiviteitenVerenigingsloket = verenigingZoekDocument.HoofdactiviteitenVerenigingsloket
                                                                      .Select(Map)
                                                                      .ToArray(),
            Locaties = verenigingZoekDocument.Locaties
                                             .Select(Map)
                                             .ToArray(),
            Sleutels = verenigingZoekDocument.Sleutels
                                             .Select(Map)
                                             .ToArray(),
            Relaties = verenigingZoekDocument.Relaties
                                             .Select(r => Map(appSettings, r))
                                             .ToArray(),
            Links = Map(verenigingZoekDocument.VCode, appSettings),
        };

    private static DoelgroepResponse Map(Doelgroep doelgroep)
        => new()
        {
            id = doelgroep.JsonLdMetadata.Id,
            type = doelgroep.JsonLdMetadata.Type,
            Minimumleeftijd = doelgroep.Minimumleeftijd,
            Maximumleeftijd = doelgroep.Maximumleeftijd,
        };

    private static VerenigingLinks Map(string vCode, AppSettings appSettings)
        => new() { Detail = new Uri($"{appSettings.BaseUrl}/v1/verenigingen/{vCode}") };

    private static HoofdactiviteitVerenigingsloket Map(VerenigingZoekDocument.HoofdactiviteitVerenigingsloket h)
        => new()
        {
            id = h.JsonLdMetadata.Id,
            type = h.JsonLdMetadata.Type,
            Code = h.Code,
            Naam = h.Naam,
        };

    private static VerenigingsType Map(VerenigingZoekDocument.VerenigingsType verenigingDocumentType)
        => new()
        {
            Code = verenigingDocumentType.Code,
            Naam = verenigingDocumentType.Naam,
        };

    private static Metadata GetMetadata(ISearchResponse<VerenigingZoekDocument> searchResponse, PaginationQueryParams paginationRequest)
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
        ISearchResponse<VerenigingZoekDocument> searchResponse,
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
    public static string AddHoofdactiviteitToQuery(
        AppSettings appSettings,
        string hoofdactiviteitenVerenigingsloketCode,
        string originalQuery,
        string[] hoofdactiviteiten)
        => $"{appSettings.BaseUrl}/v1/verenigingen/zoeken?q={originalQuery}&facets.hoofdactiviteitenVerenigingsloket={CalculateHoofdactiviteiten(hoofdactiviteiten, hoofdactiviteitenVerenigingsloketCode)}";

    private static string CalculateHoofdactiviteiten(IEnumerable<string> originalHoofdactiviteiten, string hoofdActiviteitCode)
        => string.Join(
            separator: ',',
            originalHoofdactiviteiten.Append(hoofdActiviteitCode).Select(x => x.ToUpperInvariant()).Distinct());

    private static Locatie Map(VerenigingZoekDocument.Locatie loc)
        => new()
        {
            id = loc.JsonLdMetadata.Id,
            type = loc.JsonLdMetadata.Type,
            Locatietype = new LocatieType()
            {
                id = loc.Locatietype.JsonLdMetadata.Id,
                type = loc.Locatietype.JsonLdMetadata.Type,
                Naam = loc.Locatietype.Naam,
            },
            IsPrimair = loc.IsPrimair,
            Adresvoorstelling = loc.Adresvoorstelling,
            Naam = loc.Naam,
            Postcode = loc.Postcode,
            Gemeente = loc.Gemeente,
        };

    private static Sleutel Map(VerenigingZoekDocument.Sleutel s)
        => new()
        {
            id = s.JsonLdMetadata.Id,
            type = s.JsonLdMetadata.Type,
            Bron = s.Bron,
            Waarde = s.Waarde,
            CodeerSysteem = s.CodeerSysteem,
            GestructureerdeIdentificator =
                new GestructureerdeIdentificator
                {
                    id = s.GestructureerdeIdentificator.JsonLdMetadata.Id,
                    type = s.GestructureerdeIdentificator.JsonLdMetadata.Type,
                    Nummer = s.GestructureerdeIdentificator.Nummer,
                },
        };

    private static Relatie Map(AppSettings appSettings, Schema.Search.Relatie r)
        => new()
        {
            Relatietype = r.Relatietype,
            AndereVereniging = new Relatie.GerelateerdeVereniging
            {
                KboNummer = r.AndereVereniging.KboNummer,
                VCode = r.AndereVereniging.VCode,
                Naam = r.AndereVereniging.Naam,
                Detail = !string.IsNullOrEmpty(r.AndereVereniging.VCode)
                    ? $"{appSettings.BaseUrl}/v1/verenigingen/{r.AndereVereniging.VCode}"
                    : string.Empty,
            },
        };
}
