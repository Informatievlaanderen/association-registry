namespace AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe.Examples;

using AssociationRegistry.Vereniging;
using Common;
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
                Contactgegeventype = Contactgegeventype.Email,
                IsPrimair = true,
            },
        };
}
