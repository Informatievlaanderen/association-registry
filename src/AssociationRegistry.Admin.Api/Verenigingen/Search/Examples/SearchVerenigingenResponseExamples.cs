namespace AssociationRegistry.Admin.Api.Verenigingen.Search.Examples;

using Infrastructure.ConfigurationBindings;
using ResponseModels;
using Schema.Constants;
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
            Context = $"{_appSettings.PublicApiBaseUrl}/v1/contexten/beheer/zoek-verenigingen-context.json",
            Verenigingen = new[]
            {
                new Vereniging
                {
                    VCode = "V0001001",
                    Naam = "FWA De vrolijke BA’s",
                    KorteNaam = "DVB",
                    HoofdactiviteitenVerenigingsloket = new[] { new HoofdactiviteitVerenigingsloket { Code = "CULT", Beschrijving = "Cultuur" } },
                    Status = VerenigingStatus.Actief,
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
                            Naam = "",
                            Postcode = "1770",
                            Gemeente = "Liedekerke",
                        },
                    },
                    Verenigingstype = new VerenigingsType
                    {
                        Code = Verenigingstype.FeitelijkeVereniging.Code,
                        Naam = Verenigingstype.FeitelijkeVereniging.Naam,
                    },
                    Sleutels = Array.Empty<Sleutel>(),
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
                    Roepnaam = "Bronneke",
                    Status = VerenigingStatus.Actief,
                    HoofdactiviteitenVerenigingsloket = new[]
                    {
                        new HoofdactiviteitVerenigingsloket
                        {
                            Code = "SPRT",
                            Beschrijving = "Sport",
                        },
                    },
                    Verenigingstype = new VerenigingsType
                    {
                        Code = Verenigingstype.VZW.Code,
                        Naam = Verenigingstype.VZW.Naam,
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
                    Sleutels = new []
                        {
                            new Sleutel
                            {
                                Waarde = "0123456789",
                                Bron = Sleutelbron.Kbo.Waarde,
                            },
                        },
                    Links = new VerenigingLinks
                    {
                        Detail = new Uri($"{_appSettings.BaseUrl}/verenigingen/V0036651"),
                    },
                },
            },
            Metadata = new Metadata
            {
                Pagination = new Pagination { TotalCount = 2, Offset = 0, Limit = 50 },
            },
        };
}
