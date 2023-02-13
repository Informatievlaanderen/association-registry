namespace AssociationRegistry.Acm.Api.VerenigingenPerInsz.Examples;

using System.Collections.Immutable;
using Swashbuckle.AspNetCore.Filters;

public class VerenigingenResponseExamples : IExamplesProvider<VerenigingenPerInszResponse>
{
    public VerenigingenPerInszResponse GetExamples() =>
        new(
            "12345678901",
            ImmutableArray.Create(
                new Vereniging("V1234567", "FWA De vrolijke BA’s"),
                new Vereniging("V7654321", "FWA De Bron"))
        );
}
