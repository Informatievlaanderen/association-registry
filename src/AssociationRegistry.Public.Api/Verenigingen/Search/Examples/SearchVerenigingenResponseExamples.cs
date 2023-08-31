namespace AssociationRegistry.Public.Api.Verenigingen.Search.Examples;

using Infrastructure.ConfigurationBindings;
using ResponseModels;
using Swashbuckle.AspNetCore.Filters;
using System;
using Vereniging;
using HoofdactiviteitVerenigingsloket = ResponseModels.HoofdactiviteitVerenigingsloket;
using Locatie = ResponseModels.Locatie;
using Vereniging = ResponseModels.Vereniging;

public class SearchVerenigingenResponseExamples : IExamplesProvider<SearchVerenigingenResponse>
{
    private readonly AppSettings _appSettings;

    public SearchVerenigingenResponseExamples(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public SearchVerenigingenResponse GetExamples()
        => new()
        {
            Context = $"{_appSettings.BaseUrl}/v1/contexten/zoek-verenigingen-context.json",
            Verenigingen = new[]
            {
                new Vereniging
                {
                    VCode = "V0001001",
                    Naam = "FWA De vrolijke BA’s",
                    KorteNaam = "DVB",
                    Type = new VerenigingsType
                    {
                        Code = Verenigingstype.FeitelijkeVereniging.Code,
                        Beschrijving = Verenigingstype.FeitelijkeVereniging.Beschrijving,
                    },
                    HoofdactiviteitenVerenigingsloket = new[] { new HoofdactiviteitVerenigingsloket { Code = "CULT", Beschrijving = "Cultuur" } },
                    Doelgroep = new DoelgroepResponse
                    {
                        Minimumleeftijd = 0,
                        Maximumleeftijd = 150,
                    },
                    Locaties = new[]
                    {
                        new Locatie
                        {
                            Locatietype = "Correspondentie",
                            IsPrimair = true,
                            Adresvoorstelling = "kerkstraat 5, 1770 Liedekerke, Belgie",
                            Naam = null,
                            Postcode = "1770",
                            Gemeente = "Liedekerke",
                        },
                    },
                    Links = new VerenigingLinks
                    {
                        Detail = new Uri($"{_appSettings.BaseUrl}/verenigingen/V0001001"),
                    },
                },
                new Vereniging
                {
                    VCode = "V0036651",
                    Naam = "FWA De Bron",
                    Roepnaam = "Bronneke",
                    KorteNaam = string.Empty,
                    Type = new VerenigingsType
                    {
                        Code = Verenigingstype.VZW.Code,
                        Beschrijving = Verenigingstype.VZW.Beschrijving,
                    },
                    HoofdactiviteitenVerenigingsloket = new[]
                    {
                        new HoofdactiviteitVerenigingsloket
                        {
                            Code = "SPRT",
                            Beschrijving = "Sport",
                        },
                    },
                    Doelgroep = new DoelgroepResponse
                    {
                        Minimumleeftijd = 0,
                        Maximumleeftijd = 150,
                    },
                    Locaties = new[]
                    {
                        new Locatie
                        {
                            Locatietype = "Activiteiten",
                            IsPrimair = false,
                            Adresvoorstelling = "dorpstraat 91, 9000 Gent, Belgie",
                            Naam = "Cursuszaal",
                            Postcode = "9000",
                            Gemeente = "Gent",
                        },
                    },
                    Links = new VerenigingLinks
                    {
                        Detail = new Uri($"{_appSettings.BaseUrl}/verenigingen/V0036651"),
                    },
                },
            },
            Facets = new Facets
            {
                HoofdactiviteitenVerenigingsloket = new[]
                {
                    new HoofdactiviteitVerenigingsloketFacetItem
                    {
                        Code = "CULT",
                        Aantal = 1,
                        Query = $"{_appSettings.BaseUrl}/verenigingen/search/q=(hoofdactiviteitVerenigingsloket.code:CULT)",
                    },
                    new HoofdactiviteitVerenigingsloketFacetItem
                    {
                        Code = "SPRT",
                        Aantal = 1,
                        Query = $"{_appSettings.BaseUrl}/verenigingen/search/q=(hoofdactiviteitVerenigingsloket.code:SPRT)",
                    },
                },
            },
            Metadata = new Metadata
            {
                Pagination = new Pagination { TotalCount = 2, Offset = 0, Limit = 50 },
            },
        };
}
