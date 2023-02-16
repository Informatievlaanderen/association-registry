namespace AssociationRegistry.Acm.Api.VerenigingenPerInsz.Examples;

using System.Collections.Generic;
using Schema.VerenigingenPerInsz;
using Swashbuckle.AspNetCore.Filters;
using VCodes;

public class VerenigingenResponseExamples : IExamplesProvider<VerenigingenPerInszDocument>
{
    public VerenigingenPerInszDocument GetExamples()
        => new()
        {
            Insz = "12345678901",
            Verenigingen = new List<Vereniging>()
            {
                new() { VCode = "V1234567", Naam = "FWA De vrolijke BA’s" },
                new() { VCode = "V7654321", Naam = "FWA De Bron" },
            },
        };
}
