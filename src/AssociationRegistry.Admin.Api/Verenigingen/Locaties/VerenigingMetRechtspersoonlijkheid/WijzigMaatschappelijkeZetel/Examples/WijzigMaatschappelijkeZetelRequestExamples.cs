namespace AssociationRegistry.Admin.Api.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel.Examples;

using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;
using Vereniging;
using RequestModels;
using Swashbuckle.AspNetCore.Filters;

public class WijzigMaatschappelijkeZetelRequestExamples : IExamplesProvider<WijzigMaatschappelijkeZetelRequest>
{
    public WijzigMaatschappelijkeZetelRequest GetExamples()
        => new()
        {
            Naam = "Naam locatie",
            IsPrimair = true,
        };
}
