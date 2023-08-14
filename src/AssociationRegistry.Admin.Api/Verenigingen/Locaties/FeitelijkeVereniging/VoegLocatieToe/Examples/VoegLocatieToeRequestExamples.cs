namespace AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe.Examples;

using Common;
using Vereniging;
using RequestModels;
using Swashbuckle.AspNetCore.Filters;
using AdresId = Common.AdresId;
using Adres = Common.Adres;

public class VoegLocatieToeRequestExamples : IExamplesProvider<VoegLocatieToeRequest>
{
    public VoegLocatieToeRequest GetExamples()
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
