namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Search;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Schema.Search;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Vereniging.Mappers;
using Elastic.Clients.Elasticsearch;
using RequestModels;
using ResponseModels;
using Doelgroep = Schema.Search.VerenigingZoekDocument.Types.Doelgroep;
using HoofdactiviteitVerenigingsloket = ResponseModels.HoofdactiviteitVerenigingsloket;
using Lidmaatschap = ResponseModels.Lidmaatschap;
using Locatie = ResponseModels.Locatie;
using SubverenigingVan = ResponseModels.SubverenigingVan;
using Vereniging = ResponseModels.Vereniging;
using Verenigingssubtype = ResponseModels.Verenigingssubtype;
using Verenigingstype = ResponseModels.Verenigingstype;
using Werkingsgebied = ResponseModels.Werkingsgebied;

public class SearchVerenigingenResponseMapper
{
    private readonly AppSettings _appSettings;
    private readonly IVerenigingstypeMapper _verenigingstypeMapper;

    public SearchVerenigingenResponseMapper(AppSettings appSettings,string version)
    {
        _appSettings = appSettings;
        _verenigingstypeMapper = version == WellknownVersions.V2 ? new VerenigingstypeMapperV2() : new VerenigingstypeMapperV1();

    }

    public SearchVerenigingenResponse ToSearchVereningenResponse(
        ILogger<SearchVerenigingenController> logger,
        SearchResponse<VerenigingZoekDocument> searchResponse,
        PaginationQueryParams paginationRequest,
        string originalQuery)
        => new()
        {
            Context = $"{_appSettings.PublicApiBaseUrl}/v1/contexten/beheer/zoek-verenigingen-context.json",
            Verenigingen = searchResponse.Hits
                                         .Select(x => Map(logger, x.Source, _appSettings))
                                         .ToArray(),
            Metadata = GetMetadata(searchResponse, paginationRequest),
        };

    private Vereniging Map(
        ILogger<SearchVerenigingenController> logger,
        VerenigingZoekDocument verenigingZoekDocument,
        AppSettings appSettings)
    {
        try
        {
            return new Vereniging
            {
                type = verenigingZoekDocument.JsonLdMetadataType,
                VCode = verenigingZoekDocument.VCode,
                CorresponderendeVCodes = verenigingZoekDocument.CorresponderendeVCodes,
                Verenigingstype =
                    _verenigingstypeMapper.Map<Verenigingstype, VerenigingZoekDocument.Types.VerenigingsType>(
                        verenigingZoekDocument.Verenigingstype),
                Verenigingssubtype =
                    _verenigingstypeMapper.MapSubtype<Verenigingssubtype, VerenigingZoekDocument.Types.Verenigingssubtype>(
                        verenigingZoekDocument.Verenigingssubtype),
                SubverenigingVan = _verenigingstypeMapper.MapSubverenigingVan(verenigingZoekDocument.Verenigingssubtype,
                    () => new SubverenigingVan
                    {
                        AndereVereniging = verenigingZoekDocument.SubverenigingVan.AndereVereniging,
                        Identificatie = verenigingZoekDocument.SubverenigingVan.Identificatie,
                        Beschrijving = verenigingZoekDocument.SubverenigingVan.Beschrijving,
                    }),
                Naam = verenigingZoekDocument.Naam,
                Roepnaam = verenigingZoekDocument.Roepnaam,
                KorteNaam = verenigingZoekDocument.KorteNaam,
                Status = verenigingZoekDocument.Status,
                Startdatum = verenigingZoekDocument.Startdatum,
                Einddatum = verenigingZoekDocument.Einddatum,
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
                Lidmaatschappen = verenigingZoekDocument
                                 .Lidmaatschappen
                                 .Select(Map)
                                 .ToArray(),
                Sleutels = verenigingZoekDocument.Sleutels
                                                 .Select(Map)
                                                 .ToArray(),
                Links = Map(verenigingZoekDocument.VCode, appSettings),
            };
        }
        catch (Exception e)
        {
            logger.LogError(message: "Could not map {VCode}: \n{@Doc}", verenigingZoekDocument.VCode, verenigingZoekDocument);

            throw;
        }
    }

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

    private static Werkingsgebied Map(VerenigingZoekDocument.Types.Werkingsgebied wg)
        => new()
        {
            id = wg.JsonLdMetadata.Id,
            type = wg.JsonLdMetadata.Type,
            Code = wg.Code,
            Naam = wg.Naam,
        };

    private static Metadata GetMetadata(SearchResponse<VerenigingZoekDocument> searchResponse, PaginationQueryParams paginationRequest)
        => new()
        {
            Pagination = new Pagination
            {
                TotalCount = searchResponse.Total,
                Offset = paginationRequest.Offset,
                Limit = paginationRequest.Limit,
            },
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

    private static Locatie Map(VerenigingZoekDocument.Types.Locatie loc)
    {
        if (loc == null)
            throw new ArgumentNullException(nameof(loc));

        if (loc.JsonLdMetadata == null)
            throw new ArgumentNullException(nameof(loc.JsonLdMetadata));

        return new Locatie
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
    }

    private static Sleutel Map(VerenigingZoekDocument.Types.Sleutel s)
        => new()
        {
            id = s.JsonLdMetadata.Id,
            type = s.JsonLdMetadata.Type,
            Bron = s.Bron,
            Waarde = s.Waarde,
            CodeerSysteem = s.CodeerSysteem,
            GestructureerdeIdentificator = new GestructureerdeIdentificator
            {
                id = s.GestructureerdeIdentificator.JsonLdMetadata.Id,
                type = s.GestructureerdeIdentificator.JsonLdMetadata.Type,
                Nummer = s.GestructureerdeIdentificator.Nummer,
            },
        };
}
