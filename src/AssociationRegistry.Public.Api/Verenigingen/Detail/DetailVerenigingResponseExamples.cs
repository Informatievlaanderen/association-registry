namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

using System;
using Infrastructure.ConfigurationBindings;
using ResponseModels;
using Swashbuckle.AspNetCore.Filters;

public class DetailVerenigingResponseExamples : IExamplesProvider<PubliekVerenigingDetailResponse>
{
    private readonly AppSettings _appSettings;

    public DetailVerenigingResponseExamples(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public PubliekVerenigingDetailResponse GetExamples()
        => new()
        {
            Context = $"{_appSettings.BaseUrl}/v1/contexten/detail-vereniging-context.json",
            Vereniging = new Vereniging
            {
                VCode = "V0001001",
                Type = new VerenigingsType
                {
                    Code = "FV",
                    Beschrijving = "Feitelijke vereniging",
                },
                Naam = "FWA De vrolijke BA’s",
                KorteNaam = "DVB",
                KorteBeschrijving = "De vereniging van de vrolijke BA's",
                Startdatum = new DateOnly(2020, 05, 15),
                Doelgroep = new DoelgroepResponse
                {
                    Minimumleeftijd = 0,
                    Maximumleeftijd = 150,
                },Status = "Actief",
                Contactgegevens = new[]
                {
                    new Contactgegeven
                    {
                        Type = "E-mail",
                        Waarde = "info@example.org",
                        Beschrijving = "Info",
                        IsPrimair = false,
                    },
                },
                Locaties = new[]
                {
                    new Locatie
                    {
                        Locatietype = "Correspondentie",
                        IsPrimair = true,
                        Adresvoorstelling = "kerkstraat 5, 1770 Liedekerke, Belgie",
                        Naam = "de kerk",
                        Adres = new Adres
                        {
                            Straatnaam = "kerkstraat",
                            Huisnummer = "5",
                            Busnummer = null,
                            Postcode = "1770",
                            Gemeente = "Liedekerke",
                            Land = "Belgie",
                        },
                    },
                },
                HoofdactiviteitenVerenigingsloket = new[]
                {
                    new HoofdactiviteitVerenigingsloket
                    {
                        Code = "CULT",
                        Beschrijving = "Cultuur",
                    },
                },
                Sleutels = Array.Empty<Sleutel>(),
                Relaties = Array.Empty<Relatie>(),
            },
            Metadata = new Metadata { DatumLaatsteAanpassing = "2023-05-15" },
        };
}
