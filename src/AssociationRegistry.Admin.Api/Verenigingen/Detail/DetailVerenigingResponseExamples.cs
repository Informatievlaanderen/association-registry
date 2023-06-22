namespace AssociationRegistry.Admin.Api.Verenigingen.Detail;

using System;
using Infrastructure.ConfigurationBindings;
using ResponseModels;
using Swashbuckle.AspNetCore.Filters;
using Vereniging;
using Adres = ResponseModels.Adres;
using Contactgegeven = ResponseModels.Contactgegeven;
using HoofdactiviteitVerenigingsloket = ResponseModels.HoofdactiviteitVerenigingsloket;
using Locatie = ResponseModels.Locatie;
using Vertegenwoordiger = ResponseModels.Vertegenwoordiger;

public class DetailVerenigingResponseExamples : IExamplesProvider<DetailVerenigingResponse>
{
    private readonly AppSettings _appSettings;

    public DetailVerenigingResponseExamples(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public DetailVerenigingResponse GetExamples()
        => new()
        {
            Context = $"{_appSettings.BaseUrl}/v1/contexten/detail-vereniging-context.json",
            Vereniging = new VerenigingDetail
            {
                VCode = "V0001001",
                Type = new VerenigingsType
                {
                    Beschrijving = Verenigingstype.FeitelijkeVereniging.Beschrijving,
                    Code = Verenigingstype.FeitelijkeVereniging.Code,
                },
                Naam = "FWA De vrolijke BA’s",
                KorteNaam = "DVB",
                KorteBeschrijving = "De vereniging van de vrolijke BA's",
                Startdatum = "2020-05-15",
                Status = "Actief",
                IsUitgeschrevenUitPubliekeDatastroom = false,
                HoofdactiviteitenVerenigingsloket = new[]
                {
                    new HoofdactiviteitVerenigingsloket
                    {
                        Code = "CULT",
                        Beschrijving = "Cultuur",
                    },
                },
                Contactgegevens = new[]
                {
                    new Contactgegeven
                    {
                        Type = "Email",
                        Beschrijving = "Info",
                        Waarde = "info@example.org",
                        ContactgegevenId = 1,
                        IsPrimair = false,
                    },
                },
                Locaties = new[]
                {
                    new Locatie
                    {
                        LocatieId = 1,
                        Locatietype = "Correspondentie",
                        Hoofdlocatie = true,
                        AdresWeergave = "kerkstraat 5, 1770 Liedekerke, Belgie",
                        Naam = null,
                        Adres = new Adres
                        {
                        Postcode = "1770",
                        Gemeente = "Liedekerke"},
                    },
                },
                Vertegenwoordigers = new[]
                {
                    new Vertegenwoordiger
                    {
                        VertegenwoordigerId = 1,
                        Voornaam = "Jhon",
                        Achternaam = "Doo",
                        PrimairContactpersoon = false,
                        Roepnaam = "Jhony",
                        Rol = "Voorzitter",
                        Email = "jhon@example.org",
                        Mobiel = "0000112233",
                        Telefoon = "0001112233",
                        SocialMedia = "http://example.org",
                    },
                },
                Sleutels = Array.Empty<Sleutel>(),
            },
            Metadata = new MetadataDetail
            {
                DatumLaatsteAanpassing = "2020-05-15",
                BeheerBasisUri = "/verenigingen/V0001001",
            },
        };
}
