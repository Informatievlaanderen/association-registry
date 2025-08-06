namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.Examples;

using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging;
using RequestModels;
using Swashbuckle.AspNetCore.Filters;
using Adres = Common.Adres;
using AdresId = Common.AdresId;

public class WijzigLocatieRequestExamples : IExamplesProvider<WijzigLocatieRequest>
{
    public WijzigLocatieRequest GetExamples()
        => new()
        {
            Locatie = new TeWijzigenLocatie
            {
                Naam = "Naam locatie",
                IsPrimair = true,
                AdresId = new AdresId
                {
                    Broncode = "AR",
                    Bronwaarde = "https://data.vlaanderen.be/id/adres/0",
                },
                Adres = new Adres
                {
                    Busnummer = "12",
                    Gemeente = "Gemeente",
                    Huisnummer = "234",
                    Land = "België",
                    Postcode = "1000",
                    Straatnaam = "Straatnaam",
                },
                Locatietype = Locatietype.Activiteiten,
            },
        };
}
