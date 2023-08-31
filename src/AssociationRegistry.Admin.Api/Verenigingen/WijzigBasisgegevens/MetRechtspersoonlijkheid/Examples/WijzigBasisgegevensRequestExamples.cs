namespace AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.Examples;

using Common;
using System.Linq;
using Vereniging;
using RequestModels;
using Swashbuckle.AspNetCore.Filters;

public class WijzigBasisgegevensRequestExamples : IExamplesProvider<WijzigBasisgegevensRequest>
{
    public WijzigBasisgegevensRequest GetExamples()
        => new()
        {
            KorteBeschrijving = "Korte beschrijving van de vereniging",
            Doelgroep = new DoelgroepRequest
            {
                Maximumleeftijd = 150,
                Minimumleeftijd = 0,
            },
            HoofdactiviteitenVerenigingsloket = new[]
            {
                HoofdactiviteitVerenigingsloket.All().First().Code,
            },
        };
}
