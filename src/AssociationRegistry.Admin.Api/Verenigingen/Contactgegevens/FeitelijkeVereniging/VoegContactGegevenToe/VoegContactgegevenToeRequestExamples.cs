namespace AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Vereniging;
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
