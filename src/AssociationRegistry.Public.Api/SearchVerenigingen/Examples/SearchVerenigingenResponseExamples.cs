namespace AssociationRegistry.Public.Api.SearchVerenigingen.Examples;

using System;
using System.Collections.Immutable;
using Extensions;
using Swashbuckle.AspNetCore.Filters;

public class SearchVerenigingenResponseExamples : IExamplesProvider<SearchVerenigingenResponse>
{
    public SearchVerenigingenResponse GetExamples()
        => new()
        {
            Verenigingen = ImmutableArray.Create(
                new Vereniging(
                    "V123456",
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
                    new VerenigingLinks(new Uri("https://???/verenigingen/v123456"))),
                new Vereniging(
                    "V765432",
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
                    new VerenigingLinks(new Uri("https://???/verenigingen/v765432"))
                )),
            Facets = new Facets
            {
                HoofdActiviteiten =
                    ImmutableDictionary.Create<string, long>()
                        .Add("Cultuur", 1)
                        .Add("Sport", 1),
            },
            Metadata = new Metadata(new Pagination(2, 0, 50)),
        };
}
