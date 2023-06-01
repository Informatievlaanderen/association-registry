namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling;

using System;
using Constants;
using Common;
using Vereniging;
using DecentraalBeheerdeVereniging;
using Swashbuckle.AspNetCore.Filters;

public class RegistreerAfdelingRequestExamples : IExamplesProvider<RegistreerDecentraalBeheerdeVerenigingRequest>
{
    public RegistreerDecentraalBeheerdeVerenigingRequest GetExamples()
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
