namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.InStopzetting.Examples;

using RequestModels;
using Swashbuckle.AspNetCore.Filters;

public class InStopzettingRequestExample : IExamplesProvider<InStopzettingRequest>
{
    public InStopzettingRequest GetExamples() => new() { InStopzetting = true };
}
