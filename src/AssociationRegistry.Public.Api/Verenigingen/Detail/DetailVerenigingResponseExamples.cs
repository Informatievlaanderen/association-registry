namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

using System;
using ResponseModels;
using Swashbuckle.AspNetCore.Filters;

public class DetailVerenigingResponseExamples : IExamplesProvider<PubliekVerenigingDetailResponse>
{
    public PubliekVerenigingDetailResponse GetExamples()
        => new()
        {
            Context = "",
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
                Status = "Actief",
                Contactgegevens = new[]
                {
                    new Contactgegeven
                    {
                        Type = "Email",
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
                        Hoofdlocatie = true,
                        Adres = "kerkstraat 5, 1770 Liedekerke, Belgie",
                        Naam = "de kerk",
                        Straatnaam = "kerkstraat",
                        Huisnummer = "5",
                        Busnummer = null,
                        Postcode = "1770",
                        Gemeente = "Liedekerke",
                        Land = "Belgie",
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
            },
            Metadata = new Metadata { DatumLaatsteAanpassing = "2023-05-15" },
        };
}
