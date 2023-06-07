namespace AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid;

using System.Linq;
using AssociationRegistry.Vereniging;
using Swashbuckle.AspNetCore.Filters;

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
