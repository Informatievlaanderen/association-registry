namespace AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.WijzigContactgegeven;

using Swashbuckle.AspNetCore.Filters;

public class WijzigContactgegevenRequestExamples : IExamplesProvider<WijzigContactgegevenRequest>
{
    public WijzigContactgegevenRequest GetExamples()
        => new()
        {
            Contactgegeven = new WijzigContactgegevenRequest.TeWijzigenContactgegeven
            {
                Beschrijving = "Algemeen",
                Waarde = "algemeen@example.com",
                IsPrimair = true,
            },
        };
}
