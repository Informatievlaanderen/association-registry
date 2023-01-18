namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using System;
using Constants;
using Swashbuckle.AspNetCore.Filters;

public class RegistreerVerenigingenRequestExamples : IExamplesProvider<RegistreerVerenigingRequest>
{
    public RegistreerVerenigingRequest GetExamples()
        => new() { Naam = "Naam van de vereniging", Initiator = "OVO000001", KorteNaam = "Korte naam", KorteBeschrijving = "Beschrijving", Locaties = new []
        {
            new RegistreerVerenigingRequest.Locatie()
            {
                Naam = "Naam locatie",
                Busnummer = "12",
                Gemeente = "Gemeente",
                Hoofdlocatie = true,
                Huisnummer = "234",
                Land = "België",
                Locatietype = Locatietypes.Activiteiten,
                Postcode = "1000",
                Straatnaam = "Straatnaam",
            },
        }, KboNummer = "BE0123456789",
            StartDatum = DateOnly.FromDateTime(DateTime.Today),
            ContactInfoLijst = new []
            {
                new RegistreerVerenigingRequest.ContactInfo()
                {
                    Contactnaam = "Algemeen",
                    Email = "algemeen@example.com",
                    Telefoon = "000000000",
                    Website = "https://example.com",
                    SocialMedia = "@example",
                },
            },
        };
}
