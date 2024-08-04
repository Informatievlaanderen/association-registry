namespace AssociationRegistry.Public.Api.Verenigingen.Search.Examples;

using Infrastructure.ConfigurationBindings;
using JsonLdContext;
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
            Context = $"{_appSettings.BaseUrl}/v1/contexten/publiek/zoek-verenigingen-context.json",
            Verenigingen = new[]
            {
                new Vereniging
                {
                    type = JsonLdType.FeitelijkeVereniging.Type, VCode = "V0001001",
                    Naam = "FWA De vrolijke BA’s",
                    KorteNaam = "DVB",
                    KorteBeschrijving = "Een vrolijke groep van BA'ers die graag BA dingen doen.",
                    Verenigingstype = new VerenigingsType
                    {
                        Code = Verenigingstype.FeitelijkeVereniging.Code,
                        Naam = Verenigingstype.FeitelijkeVereniging.Naam,
                    },
                    HoofdactiviteitenVerenigingsloket = new[]
                    {
                        new HoofdactiviteitVerenigingsloket
                        {
                            id = JsonLdType.Hoofdactiviteit.CreateWithIdValues("CULT"),
                            type = JsonLdType.Hoofdactiviteit.Type,
                            Code = "CULT", Naam = "Cultuur",
                        },
                    },
                    Doelgroep = new DoelgroepResponse
                    {
                        id = JsonLdType.Doelgroep.CreateWithIdValues("V0001001"),
                        type = JsonLdType.Doelgroep.Type,
                        Minimumleeftijd = 0,
                        Maximumleeftijd = 150,
                    },
                    Locaties = new[]
                    {
                        new Locatie
                        {
                            id = JsonLdType.Locatie.CreateWithIdValues("V0001001", "1"),
                            type = JsonLdType.Locatie.Type,
                            Locatietype = Locatietype.Correspondentie.Waarde,
                            IsPrimair = true,
                            Adresvoorstelling = "kerkstraat 5, 1770 Liedekerke, België",
                            Naam = "",
                            Postcode = "1770",
                            Gemeente = "Liedekerke",
                        },
                    },
                    Sleutels =
                        new[]
                        {
                            new Sleutel
                            {
                                id = JsonLdType.Sleutel.CreateWithIdValues("V0001001", Sleutelbron.VR),
                                type = JsonLdType.Sleutel.Type,
                                Bron = Sleutelbron.VR,
                                Waarde = "V0001001",
                                CodeerSysteem = CodeerSysteem.VR,
                                GestructureerdeIdentificator = new GestructureerdeIdentificator
                                {
                                    id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues("V0001001", Sleutelbron.VR),
                                    type = JsonLdType.GestructureerdeSleutel.Type,
                                    Nummer = "V0001001",
                                },
                            },
                        },
                    Relaties = Array.Empty<Relatie>(),
                    Links = new VerenigingLinks
                    {
                        Detail = new Uri($"{_appSettings.BaseUrl}/verenigingen/V0001001"),
                    },
                },
                new Vereniging
                {
                    type = JsonLdType.FeitelijkeVereniging.Type,
                    VCode = "V0036651",
                    Naam = "FWA De Bron",
                    Roepnaam = "Bronneke",
                    KorteNaam = string.Empty,
                    KorteBeschrijving = string.Empty,
                    Verenigingstype = new VerenigingsType
                    {
                        Code = Verenigingstype.VZW.Code,
                        Naam = Verenigingstype.VZW.Naam,
                    },
                    HoofdactiviteitenVerenigingsloket = new[]
                    {
                        new HoofdactiviteitVerenigingsloket
                        {
                            id = JsonLdType.Hoofdactiviteit.CreateWithIdValues("SPRT"),
                            type = JsonLdType.Hoofdactiviteit.Type,
                            Code = "SPRT",
                            Naam = "Sport",
                        },
                    },
                    Doelgroep = new DoelgroepResponse
                    {
                        id = JsonLdType.Doelgroep.CreateWithIdValues("V0036651"),
                        type = JsonLdType.Doelgroep.Type, Minimumleeftijd = 0,
                        Maximumleeftijd = 150,
                    },
                    Locaties = new[]
                    {
                        new Locatie
                        {
                            id = JsonLdType.Locatie.CreateWithIdValues("V0036651", "1"),
                            type = JsonLdType.Locatie.Type,
                            Locatietype = Locatietype.Activiteiten.Waarde,
                            IsPrimair = false,
                            Adresvoorstelling = "dorpstraat 91, 9000 Gent, België",
                            Naam = "Cursuszaal",
                            Postcode = "9000",
                            Gemeente = "Gent",
                        },
                    },
                    Links = new VerenigingLinks
                    {
                        Detail = new Uri($"{_appSettings.BaseUrl}/verenigingen/V0036651"),
                    },
                    Sleutels = new[]
                    {
                        new Sleutel
                        {
                            id = JsonLdType.Sleutel.CreateWithIdValues("V0036651", Sleutelbron.KBO.Waarde),
                            type = JsonLdType.Sleutel.Type,
                            Waarde = "0123456789",
                            Bron = Sleutelbron.KBO.Waarde,
                            CodeerSysteem = CodeerSysteem.KBO,
                            GestructureerdeIdentificator = new GestructureerdeIdentificator
                            {
                                id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues("V0036651", Sleutelbron.KBO.Waarde),
                                type = JsonLdType.GestructureerdeSleutel.Type,
                                Nummer = "0123456789",
                            },
                        },new Sleutel
                        {
                            id = JsonLdType.Sleutel.CreateWithIdValues("V0036651", Sleutelbron.VR),
                            type = JsonLdType.Sleutel.Type,
                            Bron = Sleutelbron.VR,
                            Waarde = "V0036651",
                            CodeerSysteem = CodeerSysteem.VR,
                            GestructureerdeIdentificator = new GestructureerdeIdentificator
                            {
                                id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues("V0036651", Sleutelbron.VR),
                                type = JsonLdType.GestructureerdeSleutel.Type,
                                Nummer = "V0036651",
                            },
                        },
                    },
                    Relaties = Array.Empty<Relatie>(),
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
