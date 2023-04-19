namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer;

using System;
using Common;
using Constants;
using Swashbuckle.AspNetCore.Filters;
using Vereniging;

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
            Startdatum = DateOnly.FromDateTime(DateTime.Today),
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
            Contactgegevens = new[]
            {
                new ToeTeVoegenContactgegeven()
                {
                    Beschrijving = "Algemeen",
                    Waarde = "algemeen@example.com",
                    Type = ContactgegevenType.Email,
                    IsPrimair = true,
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
                    Email = "conan@example.com",
                    Telefoon = "0000112233",
                    Mobiel = "9999887766",
                    SocialMedia = "http://example.org",
                },
            },
        };
}
