namespace AssociationRegistry.Public.Api.SearchVerenigingen.Examples;

using System;
using System.Collections.Immutable;
using Extensions;
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
                    "V000001",
                    "W0000001",
                    "X1",
                    "Y1001",
                    "FWA De vrolijke BA’s",
                    "DVB",
                    new Hoofdactiviteit("CULT", "Cultuur").ObjectToImmutableArray(),
                    new Locatie(
                        "Correspondentieadres",
                        "https://data.vlaanderen.be/id/adres/2272122",
                        "1770",
                        "Liedekerke"),
                    "18+",
                    ImmutableArray.Create(
                        new Locatie(
                            "Correspondentieadres",
                            "https://data.vlaanderen.be/id/adres/2272122",
                            "1770",
                            "Liedekerke")
                    ),
                    ImmutableArray.Create(
                        new Activiteit(123, "Badminton")),
                    new VerenigingLinks(new Uri($"{_appSettings.BaseUrl}verenigingen/v123456"))),
                new Vereniging(
                    "V036651",
                    "W0036651",
                    "X36651",
                    "Y36651",
                    "FWA De Bron",
                    string.Empty,
                    new Hoofdactiviteit("SPRT", "Sport").ObjectToImmutableArray(),
                    new Locatie(
                        "Plaats van de activiteiten",
                        "https://data.vlaanderen.be/id/gemeente/44021",
                        "9000",
                        "Gent"),
                    "Alle leeftijden",
                    ImmutableArray.Create(
                        new Locatie(
                            "Plaats van de activiteiten",
                            "https://data.vlaanderen.be/id/gemeente/44021",
                            "9000",
                            "Gent")),
                    ImmutableArray.Create(
                        new Activiteit(456, "Tennis")),
                    new VerenigingLinks(new Uri($"{_appSettings.BaseUrl}verenigingen/v765432"))
                )),
            Facets = new Facets
            {
                HoofdActiviteiten = ImmutableArray.Create(
                    new HoofdActiviteitFacetItem("CULT", "Cultuur", 1, $"{_appSettings.BaseUrl}verenigingen/search/q=(hoofdactiviteiten.code:CULT)"),
                    new HoofdActiviteitFacetItem("SPRT", "Sport", 1, $"{_appSettings.BaseUrl}verenigingen/search/q=(hoofdactiviteiten.code:SPRT)")
                ),
            },
            Metadata = new Metadata(new Pagination(2, 0, 50)),
        };
}
