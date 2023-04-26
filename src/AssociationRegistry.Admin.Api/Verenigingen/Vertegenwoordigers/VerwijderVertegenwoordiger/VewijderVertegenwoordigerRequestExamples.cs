namespace AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.VerwijderVertegenwoordiger;

using Swashbuckle.AspNetCore.Filters;

public class VerwijderVertegenwoordigerRequestExamples : IExamplesProvider<VerwijderVertegenwoordigerRequest>
{
    public VerwijderVertegenwoordigerRequest GetExamples()
        => new()
        {
            Initiator = "OVO000001",
        };
}
