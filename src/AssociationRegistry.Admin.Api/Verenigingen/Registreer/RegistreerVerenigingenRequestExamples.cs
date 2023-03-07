namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using System;
using Constants;
using Primitives;
using Swashbuckle.AspNetCore.Filters;

public class RegistreerVerenigingenRequestExamples : IExamplesProvider<RegistreerVerenigingRequest>
{
    public RegistreerVerenigingRequest GetExamples()
        => new()
        {
            Naam = "Naam van de vereniging",
            Initiator = "OVO000001",
            KorteNaam = "Korte naam",
            KorteBeschrijving = "Beschrijving",
            KboNummer = "BE0123456789",
            Startdatum = NullOrEmpty<DateOnly>.Create(DateOnly.FromDateTime(DateTime.Today)),
            Locaties = new[]
            {
                new RegistreerVerenigingRequest.Locatie
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
            },
            ContactInfoLijst = new[]
            {
                new RegistreerVerenigingRequest.ContactInfo
                {
                    Contactnaam = "Algemeen",
                    Email = "algemeen@example.com",
                    Telefoon = "000000000",
                    Website = "https://example.com",
                    SocialMedia = "https://example.com/example",
                },
            },
            Vertegenwoordigers = new[]
            {
                new RegistreerVerenigingRequest.Vertegenwoordiger
                {
                    Insz = "yymmddxxxcc",
                    PrimairContactpersoon = true,
                    Roepnaam = "Conan",
                    Rol = "Barbarian",
                    ContactInfoLijst = new[]
                    {
                        new RegistreerVerenigingRequest.ContactInfo
                        {
                            Contactnaam = "Persoonlijk",
                            Email = "conan@example.com",
                            Telefoon = "000000007",
                            Website = "https://conan.com",
                            SocialMedia = "https://example.com/conan",
                            PrimairContactInfo = true,
                        },
                    },
                },
            },
        };
}
