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
            Contactgegevens = new[]
            {
                new RegistreerVerenigingRequest.Contactgegeven()
                {
                    Omschrijving = "Algemeen",
                    Waarde = "algemeen@example.com",
                    Type = RequestContactgegevenTypes.Email,
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
                    Contactgegevens = new[]
                    {
                        new RegistreerVerenigingRequest.Contactgegeven()
                        {
                            Omschrijving = "Persoonlijk",
                            Waarde = "conan@example.com",
                            Type = RequestContactgegevenTypes.Email,
                            IsPrimair = true,
                        },
                    },
                },
            },
        };
}
