namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel.Examples;

using RequestModels;
using Swashbuckle.AspNetCore.Filters;

public class WijzigMaatschappelijkeZetelRequestExamples : IExamplesProvider<WijzigMaatschappelijkeZetelRequest>
{
    public WijzigMaatschappelijkeZetelRequest GetExamples()
        => new()
        {
            Locatie = new TeWijzigenMaatschappelijkeZetel
            {
                Naam = "Naam locatie",
                IsPrimair = true,
            },
        };
}
