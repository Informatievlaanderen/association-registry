namespace AssociationRegistry.Acm.Api.VerenigingenPerInsz.Examples;

using System.Collections.Generic;
using Schema.VerenigingenPerInsz;
using Swashbuckle.AspNetCore.Filters;

public class VerenigingenResponseExamples : IExamplesProvider<VerenigingenPerInszDocument>
{
    public VerenigingenPerInszDocument GetExamples()
        => new()
        {
            Insz = "12345678901",
            Verenigingen = new List<Vereniging>()
            {
                new("V1234567", "FWA De vrolijke BA’s"),
                new("V7654321", "FWA De Bron"),
            },
        };
}
