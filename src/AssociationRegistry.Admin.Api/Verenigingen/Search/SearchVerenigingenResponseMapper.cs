﻿namespace AssociationRegistry.Admin.Api.Verenigingen.Search;

using Infrastructure.ConfigurationBindings;
using Nest;
using RequestModels;
using ResponseModels;
using Schema.Search;
using System;
using System.Collections.Generic;
using System.Linq;

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
        string originalQuery)
        => new()
        {
            Context = $"{_appSettings.PublicApiBaseUrl}/v1/contexten/beheer/zoek-verenigingen-context.json",
            Verenigingen = searchResponse.Hits
                                         .Select(x => Map(x.Source, _appSettings))
                                         .ToArray(),
            Metadata = GetMetadata(searchResponse, paginationRequest),
        };

    private static Vereniging Map(VerenigingZoekDocument verenigingZoekDocument, AppSettings appSettings)
        => new()
        {
            type = verenigingZoekDocument.JsonLdMetadataType,
            VCode = verenigingZoekDocument.VCode,
            Verenigingstype = Map(verenigingZoekDocument.Verenigingstype),
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
            Locaties = verenigingZoekDocument.Locaties
                                             .Select(Map)
                                             .ToArray(),
            Sleutels = verenigingZoekDocument.Sleutels
                                             .Select(Map)
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
            Locatietype = loc.Locatietype,
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
            GestructureerdeIdentificator = new GestructureerdeIdentificator
            {
                id = s.GestructureerdeIdentificator.JsonLdMetadata.Id,
                type = s.GestructureerdeIdentificator.JsonLdMetadata.Type,
                Nummer = s.GestructureerdeIdentificator.Nummer,
            },
        };
}
