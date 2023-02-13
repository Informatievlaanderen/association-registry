namespace AssociationRegistry.Acm.Api.VerenigingenPerInsz.Examples;

using Schema.VerenigingenPerInsz;
using Swashbuckle.AspNetCore.Filters;

public class VerenigingenResponseExamples : IExamplesProvider<VerenigingenPerInszDocument>
{
    public VerenigingenPerInszDocument GetExamples()
        => new()
        {
            Insz = "12345678901",
            Verenigingen = new[]
            {
                new AssociationRegistry.Acm.Schema.VerenigingenPerInsz.Vereniging("V1234567", "FWA De vrolijke BA’s"),
                new AssociationRegistry.Acm.Schema.VerenigingenPerInsz.Vereniging("V7654321", "FWA De Bron")
            }
        };
}
