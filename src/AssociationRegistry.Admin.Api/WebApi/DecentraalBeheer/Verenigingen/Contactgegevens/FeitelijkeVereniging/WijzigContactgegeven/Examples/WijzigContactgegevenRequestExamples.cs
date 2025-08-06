namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Contactgegevens.FeitelijkeVereniging.WijzigContactgegeven.Examples;

using RequestModels;
using Swashbuckle.AspNetCore.Filters;

public class WijzigContactgegevenRequestExamples : IExamplesProvider<WijzigContactgegevenRequest>
{
    public WijzigContactgegevenRequest GetExamples()
        => new()
        {
            Contactgegeven = new TeWijzigenContactgegeven
            {
                Beschrijving = "Algemeen",
                Waarde = "algemeen@example.com",
                IsPrimair = true,
            },
        };
}
