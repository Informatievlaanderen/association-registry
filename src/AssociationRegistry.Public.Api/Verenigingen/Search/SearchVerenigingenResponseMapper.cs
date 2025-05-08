namespace AssociationRegistry.Public.Api.Verenigingen.Search;

using Constants;
using Infrastructure;
using Infrastructure.ConfigurationBindings;
using Nest;
using RequestModels;
using ResponseModels;
using Schema.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using Vereniging.Mappers;
using Doelgroep = Schema.Search.VerenigingZoekDocument.Types.Doelgroep;
using HoofdactiviteitVerenigingsloket = ResponseModels.HoofdactiviteitVerenigingsloket;
using Lidmaatschap = ResponseModels.Lidmaatschap;
using Locatie = ResponseModels.Locatie;
using Relatie = ResponseModels.Relatie;
using SubverenigingVan = ResponseModels.SubverenigingVan;
using Vereniging = ResponseModels.Vereniging;
using Verenigingssubtype = ResponseModels.Verenigingssubtype;
using Werkingsgebied = ResponseModels.Werkingsgebied;

public class SearchVerenigingenResponseMapper
{
    private readonly AppSettings _appSettings;
    private readonly ILogger<SearchVerenigingenController> _logger;
    private readonly IVerenigingstypeMapper _verenigingstypeMapper;

    public SearchVerenigingenResponseMapper(AppSettings appSettings, ILogger<SearchVerenigingenController> logger, string? version)
    {
        _appSettings = appSettings;
        _logger = logger;
        _verenigingstypeMapper = version == WellknownVersions.V2 ? new VerenigingstypeMapperV2() : new VerenigingstypeMapperV1();
    }

    public SearchVerenigingenResponse ToSearchVereningenResponse(
        ILogger<SearchVerenigingenController> logger,
        ISearchResponse<VerenigingZoekDocument> searchResponse,
        PaginationQueryParams paginationRequest,
        string originalQuery,
        string[] hoofdactiviteiten)
    {
        return new SearchVerenigingenResponse
        {
            Context = $"{_appSettings.BaseUrl}/v1/contexten/publiek/zoek-verenigingen-context.json",
            Verenigingen = searchResponse.Hits
                                         .Select(x => Map(logger, x.Source, _appSettings))
                                         .ToArray(),
            Facets = MapFacets(searchResponse, originalQuery, hoofdactiviteiten),
            Metadata = GetMetadata(searchResponse, paginationRequest),
        };
    }

    private Facets MapFacets(ISearchResponse<VerenigingZoekDocument> searchResponse, string originalQuery, string[] hoofdactiviteiten)
        => new()
        {
            HoofdactiviteitenVerenigingsloket = GetHoofdActiviteitFacets(_appSettings, searchResponse, originalQuery, hoofdactiviteiten),
        };

    private Vereniging Map(
        ILogger<SearchVerenigingenController> logger,
        VerenigingZoekDocument verenigingZoekDocument,
        AppSettings appSettings)
    {
        if (verenigingZoekDocument == null)
            throw new ArgumentNullException(nameof(verenigingZoekDocument));

        if (appSettings == null)
            throw new ArgumentNullException(nameof(appSettings));

        try
        {
            return new Vereniging
            {
                type = verenigingZoekDocument.JsonLdMetadataType,
                VCode = verenigingZoekDocument.VCode,
                Verenigingstype =
                    _verenigingstypeMapper.Map<VerenigingsType, VerenigingZoekDocument.Types.Verenigingstype>(
                        verenigingZoekDocument.Verenigingstype),
                Verenigingssubtype =
                    _verenigingstypeMapper.MapSubtype<Verenigingssubtype, VerenigingZoekDocument.Types.Verenigingssubtype>(
                        verenigingZoekDocument.Verenigingssubtype),
                SubverenigingVan = _verenigingstypeMapper
                   .MapSubverenigingVan(verenigingZoekDocument.Verenigingssubtype, () =>
                                            new SubverenigingVan()
                                            {
                                                AndereVereniging = verenigingZoekDocument.SubverenigingVan.AndereVereniging,
                                                Identificatie = verenigingZoekDocument.SubverenigingVan.Identificatie,
                                                Beschrijving = verenigingZoekDocument.SubverenigingVan.Beschrijving,
                                            }),
                Naam = verenigingZoekDocument.Naam,
                Roepnaam = verenigingZoekDocument.Roepnaam,
                KorteNaam = verenigingZoekDocument.KorteNaam,
                KorteBeschrijving = verenigingZoekDocument.KorteBeschrijving,
                Doelgroep = Map(verenigingZoekDocument.Doelgroep),
                HoofdactiviteitenVerenigingsloket = verenigingZoekDocument.HoofdactiviteitenVerenigingsloket
                                                                          .Select(Map)
                                                                          .ToArray(),
                Werkingsgebieden = verenigingZoekDocument.Werkingsgebieden
                                                         .Select(Map)
                                                         .ToArray(),
                Locaties = verenigingZoekDocument.Locaties
                                                 .Select(Map)
                                                 .ToArray(),
                Lidmaatschappen = verenigingZoekDocument.Lidmaatschappen
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
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while mapping {VCode}", verenigingZoekDocument.VCode);

            throw;
        }
    }

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

    private static HoofdactiviteitVerenigingsloket Map(VerenigingZoekDocument.Types.HoofdactiviteitVerenigingsloket h)
        => new()
        {
            id = h.JsonLdMetadata.Id,
            type = h.JsonLdMetadata.Type,
            Code = h.Code,
            Naam = h.Naam,
        };

    private static Werkingsgebied Map(VerenigingZoekDocument.Types.Werkingsgebied w)
        => new()
        {
            id = w.JsonLdMetadata.Id,
            type = w.JsonLdMetadata.Type,
            Code = w.Code,
            Naam = w.Naam,
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

    private HoofdactiviteitVerenigingsloketFacetItem[] GetHoofdActiviteitFacets(
        AppSettings appSettings,
        ISearchResponse<VerenigingZoekDocument> searchResponse,
        string originalQuery,
        string[] hoofdactiviteiten)
    {
        if (appSettings is null) throw new ArgumentException("Search response is null", nameof(appSettings));
        if (searchResponse is null) throw new ArgumentException("Search response is null", nameof(searchResponse));

        if (searchResponse.Aggregations is null)
            throw new ArgumentException("Search response is null", nameof(searchResponse.Aggregations));

        _logger.LogInformation($"Original query: {originalQuery}");
        _logger.LogInformation(string.Join(", ", hoofdactiviteiten));

        var buckets = searchResponse.Aggregations
                                    .Filter(WellknownFacets.GlobalAggregateName)
                                    .Filter(WellknownFacets.FilterAggregateName)
                                    .Terms(WellknownFacets.HoofdactiviteitenCountAggregateName)
                                    .Buckets;

        if (buckets is null) throw new ArgumentException("Search buckets is null", nameof(buckets));

        return buckets
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

    private static Lidmaatschap Map(VerenigingZoekDocument.Types.Lidmaatschap l)
        => new()
        {
            id = l.JsonLdMetadata.Id,
            type = l.JsonLdMetadata.Type,
            AndereVereniging = l.AndereVereniging,
            Van = l.DatumVan,
            Tot = l.DatumTot,
            Beschrijving = l.Beschrijving,
            Identificatie = l.Identificatie,
        };

    private static Locatie Map(VerenigingZoekDocument.Types.Locatie loc)
        => new()
        {
            id = loc.JsonLdMetadata.Id,
            type = loc.JsonLdMetadata.Type,
            Locatietype = loc.Locatietype,
            IsPrimair = loc.IsPrimair,
            Adresvoorstelling = loc.Adresvoorstelling,
            Naam = loc.Naam,
            Postcode = loc.Postcode,
            Gemeente = loc.Gemeente,
        };

    private static Sleutel Map(VerenigingZoekDocument.Types.Sleutel s)
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

    private static Relatie Map(AppSettings appSettings, Schema.Search.VerenigingZoekDocument.Types.Relatie r)
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
