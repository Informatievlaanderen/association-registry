namespace AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.WijzigContactgegeven;

using Swashbuckle.AspNetCore.Filters;

public class WijzigContactgegevenRequestExamples : IExamplesProvider<WijzigContactgegevenRequest>
{
    public WijzigContactgegevenRequest GetExamples()
        => new()
        {
            Initiator = "OVO000001",
            Contactgegeven = new WijzigContactgegevenRequest.RequestContactgegeven
            {
                Omschrijving = "Algemeen",
                Waarde = "algemeen@example.com",
                IsPrimair = true,
            },
        };
}
