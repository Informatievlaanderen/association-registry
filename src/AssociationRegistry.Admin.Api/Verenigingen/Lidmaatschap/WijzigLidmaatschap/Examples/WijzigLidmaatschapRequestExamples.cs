namespace AssociationRegistry.Admin.Api.Verenigingen.Lidmaatschap.WijzigLidmaatschap.Examples;

using Primitives;
using RequestModels;
using Swashbuckle.AspNetCore.Filters;

public class WijzigLidmaatschapRequestExamples : IExamplesProvider<WijzigLidmaatschapRequest>
{
    public WijzigLidmaatschapRequest GetExamples()
        => new()
        {
            Beschrijving = "De beschrijving van het lidmaatschap.",
            Van = NullOrEmpty<DateOnly>.Create(new DateOnly(2024, 10, 12)),
            Tot = NullOrEmpty<DateOnly>.Create(new DateOnly(2024, 10, 10)),
            Identificatie = "0012",
        };
}
