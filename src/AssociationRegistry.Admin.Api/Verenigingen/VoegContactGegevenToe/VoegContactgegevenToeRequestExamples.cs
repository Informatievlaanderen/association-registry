namespace AssociationRegistry.Admin.Api.Verenigingen.VoegContactGegevenToe;

using Swashbuckle.AspNetCore.Filters;

public class VoegContactgegevenToeRequestExamples : IExamplesProvider<VoegContactgegevenToeRequest>
{
    public VoegContactgegevenToeRequest GetExamples()
        => new()
        {
            Initiator = "OVO000001",
            Contactgegeven = new VoegContactgegevenToeRequest.RequestContactgegeven()
            {
                Omschrijving = "Algemeen",
                Waarde = "algemeen@example.com",
                Type = "email",
                IsPrimair = true,
            },
        };
}
