namespace AssociationRegistry.Admin.Api.Verenigingen.MetRechtspersoonlijkheid.WijzigBasisgegevens;

using System.Linq;
using Swashbuckle.AspNetCore.Filters;
using Vereniging;

public class WijzigBasisgegevensRequestExamples : IExamplesProvider<WijzigBasisgegevensRequest>
{
    public WijzigBasisgegevensRequest GetExamples()
        => new()
        {
            KorteBeschrijving = "Korte beschrijving van de vereniging",
            HoofdactiviteitenVerenigingsloket = new []
            {
                HoofdactiviteitVerenigingsloket.All().First().Code,
            },
        };
}
