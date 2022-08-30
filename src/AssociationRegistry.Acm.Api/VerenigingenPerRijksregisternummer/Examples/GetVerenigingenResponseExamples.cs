using System.Collections.Immutable;
using Swashbuckle.AspNetCore.Filters;

namespace AssociationRegistry.Acm.Api.VerenigingenPerRijksregisternummer.Examples;

public class GetVerenigingenResponseExamples : IExamplesProvider<GetVerenigingenPerRijksregisternummerResponse>
{
    public GetVerenigingenPerRijksregisternummerResponse GetExamples() =>
        new(
            "12345678901",
            ImmutableArray.Create(
                new Vereniging("V1234567", "FWA De vrolijke BA’s"),
                new Vereniging("V7654321", "FWA De Bron"))
        );
}
