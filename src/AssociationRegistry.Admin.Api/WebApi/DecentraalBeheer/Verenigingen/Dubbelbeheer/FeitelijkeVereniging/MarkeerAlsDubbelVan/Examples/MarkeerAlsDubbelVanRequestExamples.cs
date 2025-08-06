namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.MarkeerAlsDubbelVan.Examples;

using RequestModels;
using Swashbuckle.AspNetCore.Filters;

public class MarkeerAlsDubbelVanRequestExamples : IExamplesProvider<MarkeerAlsDubbelVanRequest>
{
    public MarkeerAlsDubbelVanRequest GetExamples()
        => new()
        {
           IsDubbelVan = "V0001002",
        };
}
