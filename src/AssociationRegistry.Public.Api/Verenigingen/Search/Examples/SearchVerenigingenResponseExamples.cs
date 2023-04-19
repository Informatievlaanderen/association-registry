namespace AssociationRegistry.Public.Api.Verenigingen.Search.Examples;

using System;
using Infrastructure.ConfigurationBindings;
using Swashbuckle.AspNetCore.Filters;

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
            Verenigingen = new[]
            {
                new Vereniging
                {
                    VCode = "V0001001",
                    Naam = "FWA De vrolijke BA’s",
                    KorteNaam = "DVB",
                    HoofdactiviteitenVerenigingsloket = new[] { new HoofdactiviteitVerenigingsloket { Code = "CULT", Beschrijving = "Cultuur" } },
                    Doelgroep = "18+",
                    Locaties = new[]
                    {
                        new Locatie
                        {
                            Locatietype = "Correspondentie",
                            Hoofdlocatie = true,
                            Adres = "kerkstraat 5, 1770 Liedekerke, Belgie",
                            Naam = null,
                            Postcode = "1770",
                            Gemeente = "Liedekerke",
                        },
                    },
                    Activiteiten = new[]
                    {
                        new Activiteit
                        {
                            Id = 123,
                            Categorie = "Badminton",
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
                    KorteNaam = string.Empty,
                    HoofdactiviteitenVerenigingsloket = new[]
                    {
                        new HoofdactiviteitVerenigingsloket
                        {
                            Code = "SPRT",
                            Beschrijving = "Sport",
                        },
                    },
                    Doelgroep = "Alle leeftijden",
                    Locaties = new[]
                    {
                        new Locatie
                        {
                            Locatietype = "Activiteiten",
                            Hoofdlocatie = false,
                            Adres = "dorpstraat 91, 9000 Gent, Belgie",
                            Naam = "Cursuszaal",
                            Postcode = "9000",
                            Gemeente = "Gent",
                        },
                    },
                    Activiteiten = new[]
                    {
                        new Activiteit { Id = 456, Categorie = "Tennis" },
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
