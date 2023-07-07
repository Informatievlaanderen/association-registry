namespace AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling;

using System;
using System.Linq;
using Common;
using FeitelijkeVereniging;
using Vereniging;
using Swashbuckle.AspNetCore.Filters;
using AdresId = Common.AdresId;
using Adres = Common.Adres;

public class RegistreerAfdelingRequestExamples : IExamplesProvider<RegistreerFeitelijkeVerenigingRequest>
{
    public RegistreerFeitelijkeVerenigingRequest GetExamples()
        => new()
        {
            Naam = "Naam van de vereniging",
            KorteNaam = "Korte naam",
            KorteBeschrijving = "Beschrijving",
            Startdatum = DateOnly.FromDateTime(DateTime.Today),
            Doelgroep = new DoelgroepRequest
            {
                Minimumleeftijd = 0,
                Maximumleeftijd = 150,
            },
            HoofdactiviteitenVerenigingsloket = HoofdactiviteitVerenigingsloket
                .All().Take(5).Select(h=>h.Code).ToArray(),
            Locaties = new[]
            {
                new ToeTeVoegenLocatie
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
