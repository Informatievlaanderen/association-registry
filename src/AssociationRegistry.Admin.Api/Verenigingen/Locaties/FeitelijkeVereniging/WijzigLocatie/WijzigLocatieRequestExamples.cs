namespace AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie;

using Common;
using Vereniging;
using Swashbuckle.AspNetCore.Filters;
using AdresId = Common.AdresId;

public class WijzigLocatieRequestExamples : IExamplesProvider<WijzigLocatieRequest>
{
    public WijzigLocatieRequest GetExamples()
        => new()
        {
            Locatie = new ToeTeVoegenLocatie
            {
                Naam = "Naam locatie",
                IsPrimair = true,
                AdresId = new AdresId
                {
                    Broncode = "AR",
                    Bronwaarde = "https://data.vlaanderen.be/id/adres/0",
                },
                Adres = new ToeTeVoegenAdres
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
