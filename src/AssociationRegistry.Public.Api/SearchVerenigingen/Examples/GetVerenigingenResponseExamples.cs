using System.Collections.Immutable;
using AssociationRegistry.Public.Api.VerenigingenPerRijksregisternummer;
using Swashbuckle.AspNetCore.Filters;

namespace AssociationRegistry.Public.Api.SearchVerenigingen.Examples;

public class SearchVerenigingenResponseExamples : IExamplesProvider<SearchVerenigingenResponse>
{
    public SearchVerenigingenResponse GetExamples() =>
        new(
            ImmutableArray.Create(
                new Vereniging(
                    "V1234567",
                    "FWA De vrolijke BA’s",
                    "DVB",
                    ImmutableArray.Create(
                        new Locatie(
                            "Correspondentieadres",
                            "https://data.vlaanderen.be/id/adres/2272122",
                            "Bombardonstraat 245, 1770 Liedekerke")
                    ),
                    ImmutableArray.Create(
                        new Activiteit(123, "Badminton"))),
                new Vereniging(
                    "V7654321",
                    "FWA De Bron",
                    string.Empty,
                    ImmutableArray.Create(
                        new Locatie(
                            "Plaats van de activiteiten",
                            "https://data.vlaanderen.be/id/gemeente/44021",
                            "Gent")),
                    ImmutableArray.Create(
                        new Activiteit(456, "Tennis"))
                ))
        );
}
