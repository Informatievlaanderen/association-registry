namespace AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie;

using Common;
using Vereniging;
using Swashbuckle.AspNetCore.Filters;
using AdresId = Common.AdresId;
using Adres = Common.Adres;

public class WijzigLocatieRequestExamples : IExamplesProvider<WijzigLocatieRequest>
{
    public WijzigLocatieRequest GetExamples()
        => new()
        {
            Locatie = new WijzigLocatieRequest.TeWijzigenLocatie()
            {
                Naam = "Naam locatie",
                IsPrimair = true,
                AdresId = new Common.AdresId
                {
                    Broncode = "AR",
                    Bronwaarde = "https://data.vlaanderen.be/id/adres/0",
                },
                Adres = new Common.Adres
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
