namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.Examples;

using AssociationRegistry.Primitives;
using AssociationRegistry.Vereniging;
using Common;
using DecentraalBeheer.Vereniging;
using RequestModels;
using Swashbuckle.AspNetCore.Filters;

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
            Werkingsgebieden = ["BE2122223", "BE2411540"],
            IsUitgeschrevenUitPubliekeDatastroom = true,
        };
}
