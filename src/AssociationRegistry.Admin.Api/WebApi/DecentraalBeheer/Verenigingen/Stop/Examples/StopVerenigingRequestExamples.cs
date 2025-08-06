namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Stop.Examples;

using RequestModels;
using Swashbuckle.AspNetCore.Filters;

public class StopVerenigingRequestExamples : IExamplesProvider<StopVerenigingRequest>
{
    public StopVerenigingRequest GetExamples()
        => new()
        {
            Einddatum = new DateOnly(year: 2023, month: 09, day: 05),
        };
}
