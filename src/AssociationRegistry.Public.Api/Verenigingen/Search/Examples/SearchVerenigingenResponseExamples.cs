namespace AssociationRegistry.Public.Api.Verenigingen.Search.Examples;

using System;
using System.Collections.Immutable;
using Infrastructure.ConfigurationBindings;
using Infrastructure.Extensions;
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
            Verenigingen = ImmutableArray.Create(
                new Vereniging(
                    "V0001001",
                    "FWA De vrolijke BA’s",
                    "DVB",
                    new HoofdactiviteitVerenigingsloket("CULT", "Cultuur").ObjectToImmutableArray(),
                    "18+",
                    ImmutableArray.Create(
                        new Locatie(
                            "Correspondentie",
                            true,
                            "kerkstraat 5, 1770 Liedekerke, Belgie",
                            null,
                            "1770",
                            "Liedekerke")
                    ),
                    ImmutableArray.Create(
                        new Activiteit(123, "Badminton")),
                    new VerenigingLinks(new ($"{_appSettings.BaseUrl}/verenigingen/V0001001"))),
                new Vereniging(
                    "V0036651",
                    "FWA De Bron",
                    string.Empty,
                    new HoofdactiviteitVerenigingsloket("SPRT", "Sport").ObjectToImmutableArray(),
                    "Alle leeftijden",
                    ImmutableArray.Create(
                        new Locatie(
                            "Activiteiten",
                            false,
                            "dorpstraat 91, 9000 Gent, Belgie",
                            "Cursuszaal",
                            "9000",
                            "Gent")),
                    ImmutableArray.Create(
                        new Activiteit(456, "Tennis")),
                    new VerenigingLinks(new Uri($"{_appSettings.BaseUrl}/verenigingen/V0036651"))
                )),
            Facets = new Facets
            {
                HoofdactiviteitenVerenigingsloket = ImmutableArray.Create(
                    new HoofdactiviteitVerenigingsloketFacetItem("CULT", 1, $"{_appSettings.BaseUrl}/verenigingen/search/q=(hoofdactiviteitVerenigingsloket.code:CULT)"),
                    new HoofdactiviteitVerenigingsloketFacetItem("SPRT", 1, $"{_appSettings.BaseUrl}/verenigingen/search/q=(hoofdactiviteitVerenigingsloket.code:SPRT)")
                ),
            },
            Metadata = new Metadata(new Pagination(2, 0, 50)),
        };
}
