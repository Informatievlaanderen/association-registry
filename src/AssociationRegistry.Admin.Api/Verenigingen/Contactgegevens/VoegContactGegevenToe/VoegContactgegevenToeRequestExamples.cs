namespace AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.VoegContactGegevenToe;

using Common;
using Swashbuckle.AspNetCore.Filters;
using Vereniging;

public class VoegContactgegevenToeRequestExamples : IExamplesProvider<VoegContactgegevenToeRequest>
{
    public VoegContactgegevenToeRequest GetExamples()
        => new()
        {
            Initiator = "OVO000001",
            Contactgegeven = new ToeTeVoegenContactgegeven
            {
                Beschrijving = "Algemeen",
                Waarde = "algemeen@example.com",
                Type = ContactgegevenType.Email,
                IsPrimair = true,
            },
        };
}
