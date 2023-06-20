namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling;

using System;
using Constants;
using Common;
using FeitelijkeVereniging;
using Vereniging;
using Swashbuckle.AspNetCore.Filters;

public class RegistreerAfdelingRequestExamples : IExamplesProvider<RegistreerFeitelijkeVerenigingRequest>
{
    public RegistreerFeitelijkeVerenigingRequest GetExamples()
        => new()
        {
            Naam = "Naam van de vereniging",
            KorteNaam = "Korte naam",
            KorteBeschrijving = "Beschrijving",
            Startdatum = DateOnly.FromDateTime(DateTime.Today),
            Locaties = new[]
            {
                new ToeTeVoegenLocatie
                {
                    Naam = "Naam locatie",
                    Hoofdlocatie = true,
                    Adres = new ToeTeVoegenAdres
                    {
                        Busnummer = "12",
                        Gemeente = "Gemeente",
                        Huisnummer = "234",
                        Land = "België",
                        Postcode = "1000",
                        Straatnaam = "Straatnaam",
                    },
                    Locatietype = Locatietypes.Activiteiten,
                },
            },
            Contactgegevens = new[]
            {
                new ToeTeVoegenContactgegeven
                {
                    Beschrijving = "Algemeen",
                    Waarde = "algemeen@example.com",
                    Type = ContactgegevenType.Email,
                    IsPrimair = true,
                },
            },
            Vertegenwoordigers = new[]
            {
                new ToeTeVoegenVertegenwoordiger
                {
                    Insz = "yymmddxxxcc",
                    IsPrimair = true,
                    Roepnaam = "Conan",
                    Rol = "Barbarian",
                    Email = "conan@example.com",
                    Telefoon = "0000112233",
                    Mobiel = "9999887766",
                    SocialMedia = "http://example.org",
                },
            },
        };
}
