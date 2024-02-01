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
                    id = JsonLdType.Vereniging.CreateWithIdValues("V0001001"),
                    type = JsonLdType.Vereniging.Type, VCode = "V0001001",
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
                            Code = "CULT", Naam = "Cultuur"
                        }
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
                            id = JsonLdType.Locatie.CreateWithIdValues("V0001001", "1"),
                            type = JsonLdType.Locatie.Type,
                            Locatietype = new LocatieType()
                            {
                                id = JsonLdType.LocatieType.CreateWithIdValues(Locatietype.Correspondentie.Waarde),
                                type = JsonLdType.LocatieType.Type,
                                Naam = Locatietype.Correspondentie.Waarde,
                            },
                            IsPrimair = true,
                            Adresvoorstelling = "kerkstraat 5, 1770 Liedekerke, Belgie",
                            Naam = "",
                            Postcode = "1770",
                            Gemeente = "Liedekerke",
                        },
                    },
                    Sleutels = Array.Empty<Sleutel>(),
                    Relaties = Array.Empty<Relatie>(),
                    Links = new VerenigingLinks
                    {
                        Detail = new Uri($"{_appSettings.BaseUrl}/verenigingen/V0001001"),
                    },
                },
                new Vereniging
                {
                    id = JsonLdType.Vereniging.CreateWithIdValues("V0036651"),
                    type = JsonLdType.Vereniging.Type,
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
                        Minimumleeftijd = 0,
                        Maximumleeftijd = 150,
                    },
                    Locaties = new[]
                    {
                        new Locatie
                        {
                            id = JsonLdType.Locatie.CreateWithIdValues("V0036651", "1"),
                            type = JsonLdType.Locatie.Type,
                            Locatietype = new LocatieType()
                            {
                                id = JsonLdType.LocatieType.CreateWithIdValues(Locatietype.Activiteiten.Waarde),
                                type = JsonLdType.LocatieType.Type,
                                Naam = Locatietype.Activiteiten.Waarde,
                            },
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
                    Sleutels = new[]
                    {
                        new Sleutel
                        {
                            id = JsonLdType.Sleutel.CreateWithIdValues("V0036651", Sleutelbron.Kbo.Waarde),
                            type = JsonLdType.Sleutel.Type,
                            Waarde = "0123456789",
                            Bron = Sleutelbron.Kbo.Waarde,
                            GestructureerdeIdentificator = new GestructureerdeIdentificator
                            {
                                id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues("V0036651", Sleutelbron.Kbo.Waarde),
                                type = JsonLdType.GestructureerdeSleutel.Type,
                                Nummer = "0123456789",
                            }
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
