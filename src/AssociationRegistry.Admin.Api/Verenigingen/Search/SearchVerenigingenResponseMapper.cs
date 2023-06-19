﻿namespace AssociationRegistry.Admin.Api.Verenigingen.Search;

using System;
using System.Collections.Generic;
using System.Linq;
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
        ISearchResponse<VerenigingZoekDocument> searchResponse,
        PaginationQueryParams paginationRequest,
        string originalQuery)
        => new()
        {
            Context = $"{_appSettings.BaseUrl}/v1/contexten/zoek-verenigingen-context.json",
            Verenigingen = searchResponse.Hits
                .Select(x => Map(x.Source, _appSettings))
                .ToArray(),
            Metadata = GetMetadata(searchResponse, paginationRequest),
        };

    private static Vereniging Map(VerenigingZoekDocument verenigingZoekDocument, AppSettings appSettings)
        => new()
        {
            VCode = verenigingZoekDocument.VCode,
            Type = Map(verenigingZoekDocument.Type),
            Naam = verenigingZoekDocument.Naam,
            KorteNaam = verenigingZoekDocument.KorteNaam,
            HoofdactiviteitenVerenigingsloket = verenigingZoekDocument.HoofdactiviteitenVerenigingsloket
                .Select(Map)
                .ToArray(),
            Doelgroep = verenigingZoekDocument.Doelgroep,
            Locaties = verenigingZoekDocument.Locaties
                .Select(Map)
                .ToArray(),
            Activiteiten = verenigingZoekDocument.Activiteiten
                .Select(Map)
                .ToArray(),
            Sleutels = verenigingZoekDocument.Sleutels
                .Select(Map)
                .ToArray(),
            Links = Map(verenigingZoekDocument.VCode, appSettings),
        };

    private static VerenigingLinks Map(string vCode, AppSettings appSettings)
        => new() { Detail = new Uri($"{appSettings.BaseUrl}/v1/verenigingen/{vCode}") };

    private static Activiteit Map(string activiteit)
        => new() { Id = -1, Categorie = activiteit };

    private static HoofdactiviteitVerenigingsloket Map(VerenigingZoekDocument.HoofdactiviteitVerenigingsloket h)
        => new() { Code = h.Code, Beschrijving = h.Naam };

    private static VerenigingsType Map(VerenigingZoekDocument.VerenigingsType verenigingDocumentType)
        => new()
        {
            Code = verenigingDocumentType.Code,
            Beschrijving = verenigingDocumentType.Beschrijving,
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
    public static string AddHoofdactiviteitToQuery(AppSettings appSettings, string hoofdactiviteitenVerenigingsloketCode, string originalQuery, string[] hoofdactiviteiten)
        => $"{appSettings.BaseUrl}/v1/verenigingen/zoeken?q={originalQuery}&facets.hoofdactiviteitenVerenigingsloket={CalculateHoofdactiviteiten(hoofdactiviteiten, hoofdactiviteitenVerenigingsloketCode)}";

    private static string CalculateHoofdactiviteiten(IEnumerable<string> originalHoofdactiviteiten, string hoofdActiviteitCode)
        => string.Join(
            separator: ',',
            originalHoofdactiviteiten.Append(hoofdActiviteitCode).Select(x => x.ToUpperInvariant()).Distinct());

    private static Locatie Map(VerenigingZoekDocument.Locatie loc)
        => new()
        {
            LocatieId = loc.LocatieId,
            Locatietype = loc.Locatietype,
            Hoofdlocatie = loc.Hoofdlocatie,
            Adres = loc.Adres,
            Naam = loc.Naam,
            Postcode = loc.Postcode,
            Gemeente = loc.Gemeente,
        };


    private static Sleutel Map(VerenigingZoekDocument.Sleutel s)
        => new()
        {
            Bron = s.Bron,
            Waarde = s.Waarde,
        };
}
