namespace AssociationRegistry.Admin.Api.Verenigingen.Detail.Examples;

using System;
using System.Collections.Generic;
using Infrastructure.ConfigurationBindings;
using ResponseModels;
using Vereniging;
using Swashbuckle.AspNetCore.Filters;
using Vereniging.Bronnen;
using Adres = ResponseModels.Adres;
using Contactgegeven = ResponseModels.Contactgegeven;
using HoofdactiviteitVerenigingsloket = ResponseModels.HoofdactiviteitVerenigingsloket;
using Locatie = ResponseModels.Locatie;
using Vertegenwoordiger = ResponseModels.Vertegenwoordiger;

public class DetailVerenigingResponseExamples : IMultipleExamplesProvider<DetailVerenigingResponse>
{
    private readonly AppSettings _appSettings;

    public DetailVerenigingResponseExamples(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public IEnumerable<SwaggerExample<DetailVerenigingResponse>> GetExamples()
    {
        yield return new SwaggerExample<DetailVerenigingResponse>
        {
            Name = "Feitelijke vereniging",
            Value = new DetailVerenigingResponse
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
                    Doelgroep = new DoelgroepResponse
                    {
                        Minimumleeftijd = 0,
                        Maximumleeftijd = 150,
                    },
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
                            Type = "E-mail",
                            Beschrijving = "Info",
                            Waarde = "info@example.org",
                            ContactgegevenId = 1,
                            IsPrimair = false,
                            Bron = Bron.Initiator.Waarde,
                        },
                    },
                    Locaties = new[]
                    {
                        new Locatie
                        {
                            LocatieId = 1,
                            Locatietype = "Correspondentie",
                            IsPrimair = true,
                            Adresvoorstelling = "kerkstraat 5, 1770 Liedekerke, Belgie",
                            Naam = null,
                            Adres = new Adres
                            {
                                Postcode = "1770",
                                Gemeente = "Liedekerke",
                            },
                            Bron = Bron.Initiator,
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
                Metadata = new Metadata
                {
                    DatumLaatsteAanpassing = "2020-05-15",
                },
            },
        };

        yield return new SwaggerExample<DetailVerenigingResponse>
        {
            Name = "Vereniging met rechtspersoonlijkheid",
            Value = new DetailVerenigingResponse
            {
                Context = $"{_appSettings.BaseUrl}/v1/contexten/detail-vereniging-context.json",
                Vereniging = new VerenigingDetail
                {
                    VCode = "V0001002",
                    Type = new VerenigingsType
                    {
                        Beschrijving = Verenigingstype.VZW.Beschrijving,
                        Code = Verenigingstype.VZW.Code,
                    },
                    Naam = "Vissen achter het net",
                    KorteNaam = "VAN",
                    KorteBeschrijving = "Een kleine groep vissers",
                    Startdatum = "2002-11-15",
                    Doelgroep = new DoelgroepResponse
                    {
                        Minimumleeftijd = 60,
                        Maximumleeftijd = 90,
                    },
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
                            Type = "E-mail",
                            Beschrijving = "Info",
                            Waarde = "info@example.org",
                            ContactgegevenId = 1,
                            IsPrimair = false,
                            Bron = Bron.KBO.Waarde,
                        },
                    },
                    Locaties = new[]
                    {
                        new Locatie
                        {
                            LocatieId = 1,
                            Locatietype = Locatietype.MaatschappelijkeZetelVolgensKbo.Waarde,
                            IsPrimair = true,
                            Adresvoorstelling = "zeebank 10, 8400 Oostende, Belgie",
                            Naam = null,
                            Adres = new Adres
                            {
                                Postcode = "8400",
                                Gemeente = "Oostende",
                            },
                            Bron = Bron.KBO.Waarde,
                        },
                    },
                    Vertegenwoordigers = Array.Empty<Vertegenwoordiger>(),
                    Sleutels = new[]
                    {
                        new Sleutel
                        {
                            Waarde = "0123456789",
                            Bron = Sleutelbron.Kbo.Waarde,
                        },
                    },
                },
                Metadata = new Metadata
                {
                    DatumLaatsteAanpassing = "2020-05-15",
                },
            },
        };
    }
}
