namespace AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.Examples;

using Common;
using Primitives;
using RequestModels;
using Swashbuckle.AspNetCore.Filters;
using Vereniging;

public class WijzigBasisgegevensRequestExamples : IExamplesProvider<WijzigBasisgegevensRequest>
{
    public WijzigBasisgegevensRequest GetExamples()
        => new()
        {
            Naam = "Naam van de vereniging",
            KorteNaam = "Korte naam van de vereniging",
            KorteBeschrijving = "Korte beschrijving van de vereniging",
            Startdatum = NullOrEmpty<DateOnly>.Create(new DateOnly(year: 2023, month: 1, day: 1)),
            Doelgroep = new DoelgroepRequest
            {
                Minimumleeftijd = 12,
                Maximumleeftijd = 18,
            },
            HoofdactiviteitenVerenigingsloket = HoofdactiviteitVerenigingsloket
                                               .All().Take(5).Select(h => h.Code).ToArray(),
            Werkingsgebieden = Werkingsgebied.All.Take(5).Select(h => h.Code).ToArray(),
            IsUitgeschrevenUitPubliekeDatastroom = true,
        };
}
