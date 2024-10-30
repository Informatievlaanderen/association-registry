namespace AssociationRegistry.Admin.Api.Verenigingen.Lidmaatschap.VoegLidmaatschapToe.Examples;

using RequestModels;
using Swashbuckle.AspNetCore.Filters;

public class VoegLidmaatschapToeRequestExamples : IExamplesProvider<VoegLidmaatschapToeRequest>
{
    public VoegLidmaatschapToeRequest GetExamples()
        => new()
        {
            AndereVereniging = "V0001001",
            Beschrijving = "De beschrijving van het lidmaatschap.",
            Van = new DateOnly(2024, 10, 12),
            Tot = new DateOnly(2024, 10, 10),
            Identificatie = "0012",
        };
}
