namespace AssociationRegistry.Admin.Api.Verenigingen.Lidmaatschap.WijzigLidmaatschap.Examples;

using RequestModels;
using Swashbuckle.AspNetCore.Filters;

public class WijzigLidmaatschapRequestExamples : IExamplesProvider<WijzigLidmaatschapRequest>
{
    public WijzigLidmaatschapRequest GetExamples()
        => new()
        {
            Beschrijving = "De beschrijving van het lidmaatschap.",
            Van = new DateOnly(2024, 10, 12),
            Tot = new DateOnly(2024, 10, 10),
            Identificatie = "0012",
        };
}
