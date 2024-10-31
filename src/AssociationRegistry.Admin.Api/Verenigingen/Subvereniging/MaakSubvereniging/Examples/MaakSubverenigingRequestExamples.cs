namespace AssociationRegistry.Admin.Api.Verenigingen.Subvereniging.MaakSubvereniging.Examples;

using RequestModels;
using Swashbuckle.AspNetCore.Filters;

public class MaakSubverenigingRequestExamples : IExamplesProvider<MaakSubverenigingRequest>
{
    public MaakSubverenigingRequest GetExamples()
        => new()
        {
            AndereVereniging = "V0001001",
            Beschrijving = "De beschrijving van het lidmaatschap.",
            Identificatie = "0012",
        };
}
