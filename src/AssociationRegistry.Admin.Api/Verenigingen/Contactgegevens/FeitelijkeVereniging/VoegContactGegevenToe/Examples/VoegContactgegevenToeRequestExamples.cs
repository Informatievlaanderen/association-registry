namespace AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe.Examples;

using Common;
using Vereniging;
using RequestsModels;
using Swashbuckle.AspNetCore.Filters;

public class VoegContactgegevenToeRequestExamples : IExamplesProvider<VoegContactgegevenToeRequest>
{
    public VoegContactgegevenToeRequest GetExamples()
        => new()
        {
            Contactgegeven = new ToeTeVoegenContactgegeven
            {
                Beschrijving = "Algemeen",
                Waarde = "algemeen@example.com",
                Type = ContactgegevenType.Email,
                IsPrimair = true,
            },
        };
}
