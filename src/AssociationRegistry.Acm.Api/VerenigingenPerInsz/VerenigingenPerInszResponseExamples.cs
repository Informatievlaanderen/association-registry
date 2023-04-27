namespace AssociationRegistry.Acm.Api.VerenigingenPerInsz;

using Swashbuckle.AspNetCore.Filters;

public class VerenigingenPerInszResponseExamples : IExamplesProvider<VerenigingenPerInszResponse>
{
    public VerenigingenPerInszResponse GetExamples()
        => new()
        {
            Insz = "12345678901",
            Verenigingen = new []
            {
                new VerenigingenPerInszResponse.Vereniging { VCode = "V1234567", Naam = "FWA De vrolijke BA’s" },
                new VerenigingenPerInszResponse.Vereniging { VCode = "V7654321", Naam = "FWA De Bron" },
            },
        };
}
