namespace AssociationRegistry.Admin.Api.Verenigingen.Search;

using AssociationRegistry.Admin.Schema.Search;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using Detail;
using Infrastructure;
using Nest;
using RequestModels;
using ResponseModels;

public class SearchVerenigingenResponseMapper
{
    private readonly AppSettings _appSettings;
    private readonly IVerenigingsTypeMapper _verenigingsTypeMapper;

    public SearchVerenigingenResponseMapper(AppSettings appSettings,string version)
    {
        _appSettings = appSettings;
        _verenigingsTypeMapper = version == WellknownVersions.V2 ? new VerenigingsTypeMapperV2() : new VerenigingsTypeMapper();

    }

    public SearchVerenigingenResponse ToSearchVereningenResponse(
        ILogger<SearchVerenigingenController> logger,
        ISearchResponse<VerenigingZoekDocument> searchResponse,
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
                Verenigingstype = _verenigingsTypeMapper.Map<VerenigingsType, VerenigingZoekDocument.VerenigingsType>(verenigingZoekDocument.Verenigingstype),
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
        catch
        {
            logger.LogError(message: "Could not map {VCode}: \n{@Doc}", verenigingZoekDocument.VCode, verenigingZoekDocument);

            throw;
        }
    }

    private static Lidmaatschap Map(VerenigingZoekDocument.Lidmaatschap l)
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

    private static HoofdactiviteitVerenigingsloket Map(VerenigingZoekDocument.HoofdactiviteitVerenigingsloket h)
        => new()
        {
            id = h.JsonLdMetadata.Id,
            type = h.JsonLdMetadata.Type,
            Code = h.Code,
            Naam = h.Naam,
        };

    private static Werkingsgebied Map(VerenigingZoekDocument.Werkingsgebied wg)
        => new()
        {
            id = wg.JsonLdMetadata.Id,
            type = wg.JsonLdMetadata.Type,
            Code = wg.Code,
            Naam = wg.Naam,
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

    private static Sleutel Map(VerenigingZoekDocument.Sleutel s)
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
