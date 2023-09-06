namespace AssociationRegistry.Admin.Api.Verenigingen.Stop.Examples;

using RequestModels;
using Swashbuckle.AspNetCore.Filters;
using System;

public class StopVerenigingRequestExamples : IExamplesProvider<StopVerenigingRequest>
{
    public StopVerenigingRequest GetExamples()
        => new()
        {
            Einddatum = new DateOnly(2023, 09, 05),
        };
}
