namespace AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.Examples;

using AssociationRegistry.Vereniging;
using Common;
using RequestModels;
using Swashbuckle.AspNetCore.Filters;

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
            Werkingsgebieden = Werkingsgebied.AllExamples.Skip(10).Take(5).Select(h => h.Code).ToArray(),
        };
}
