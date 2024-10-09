namespace AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.Examples;

using Common;
using RequestModels;
using Swashbuckle.AspNetCore.Filters;
using Vereniging;

public class WijzigBasisgegevensRequestExamples : IExamplesProvider<WijzigBasisgegevensRequest>
{
    public WijzigBasisgegevensRequest GetExamples()
        => new()
        {
            KorteBeschrijving = "Korte beschrijving van de vereniging",
            Roepnaam = "Roepnaam",
            Doelgroep = new DoelgroepRequest
            {
                Minimumleeftijd = 0,
                Maximumleeftijd = 150,
            },
            HoofdactiviteitenVerenigingsloket = new[]
            {
                HoofdactiviteitVerenigingsloket.All().First().Code,
            },
            Werkingsgebieden = Werkingsgebied.All.Select(h => h.Code).ToArray(),
        };
}
