namespace AssociationRegistry.Admin.Api.Verenigingen.Lidmaatschap.VerwijderLidmaatschap.Examples;

using RequestModels;
using Swashbuckle.AspNetCore.Filters;

public class VerwijderLidmaatschapRequestExamples : IExamplesProvider<VerwijderLidmaatschapRequest>
{
    public VerwijderLidmaatschapRequest GetExamples()
        => new()
        {
            LidmaatschapId = 1,
        };
}
