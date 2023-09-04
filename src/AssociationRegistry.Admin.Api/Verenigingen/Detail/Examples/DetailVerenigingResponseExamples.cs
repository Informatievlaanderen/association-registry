﻿namespace AssociationRegistry.Admin.Api.Verenigingen.Detail.Examples;

using Infrastructure.ConfigurationBindings;
using ResponseModels;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using Vereniging;
using Vereniging.Bronnen;
using Adres = ResponseModels.Adres;
using AdresId = ResponseModels.AdresId;
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
                    Einddatum = "2021-12-12",
                    Doelgroep = new DoelgroepResponse
                    {
                        Minimumleeftijd = 0,
                        Maximumleeftijd = 150,
                    },
                    Status = "Gestopt",
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
                            Adresvoorstelling = "Kerkstraat 5, 1770 Liedekerke, Belgie",
                            Naam = "Administratief centrum",
                            Adres = new Adres
                            {
                                Straatnaam = "Kerkstraat",
                                Huisnummer = "5",
                                Busnummer = "b",
                                Postcode = "1770",
                                Gemeente = "Liedekerke",
                                Land = "België",
                            },
                            AdresId = new AdresId
                            {
                                Broncode = Adresbron.AR,
                                Bronwaarde = AssociationRegistry.Vereniging.AdresId.DataVlaanderenAdresPrefix + 1,
                            },
                            Bron = Bron.Initiator,
                        },
                    },
                    Vertegenwoordigers = new[]
                    {
                        new Vertegenwoordiger
                        {
                            VertegenwoordigerId = 1,
                            Insz = "1234567890",
                            Voornaam = "Jane",
                            Achternaam = "Doo",
                            PrimairContactpersoon = false,
                            Roepnaam = "Jhony",
                            Rol = "Voorzitter",
                            Email = "jhon@example.org",
                            Mobiel = "0000112233",
                            Telefoon = "0001112233",
                            SocialMedia = "http://example.org",
                            Bron = Bron.Initiator.Waarde,
                        },
                    },
                    Sleutels = Array.Empty<Sleutel>(),
                    Relaties = Array.Empty<Relatie>(),
                    Bron = Bron.Initiator.Waarde,
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
                    Roepnaam = "Vissen achter 't net",
                    KorteBeschrijving = "Een kleine groep vissers",
                    Startdatum = "2002-11-15",
                    Einddatum = null,
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
                        new Contactgegeven
                        {
                            Type = ContactgegevenType.SocialMedia,
                            Beschrijving = "BlubBlub",
                            Waarde = "blubblub.com/vissen",
                            ContactgegevenId = 2,
                            IsPrimair = false,
                            Bron = Bron.Initiator.Waarde,
                        },
                    },
                    Locaties = new[]
                    {
                        new Locatie
                        {
                            LocatieId = 1,
                            Locatietype = Locatietype.MaatschappelijkeZetelVolgensKbo.Waarde,
                            IsPrimair = true,
                            Adresvoorstelling = "Zeebank 10, 8400 Oostende, Belgie",
                            Naam = "",
                            Adres = new Adres
                            {
                                Straatnaam = "Zeebank",
                                Huisnummer = "10",
                                Postcode = "8400",
                                Gemeente = "Oostende",
                                Land = "België",
                            },
                            Bron = Bron.KBO.Waarde,
                        },
                        new Locatie
                        {
                            LocatieId = 2,
                            Locatietype = Locatietype.Activiteiten.Waarde,
                            IsPrimair = false,
                            Adresvoorstelling = "De pier 1, 8430 Westende",
                            Naam = "Vis plaats",
                            Adres = new Adres
                            {
                                Straatnaam = "De pier",
                                Huisnummer = "1",
                                Postcode = "8430",
                                Gemeente = "Westende",
                                Land = "België",
                            },
                            AdresId = new AdresId
                            {
                                Broncode = Adresbron.AR,
                                Bronwaarde = AssociationRegistry.Vereniging.AdresId.DataVlaanderenAdresPrefix + 17,
                            },
                            Bron = Bron.Initiator.Waarde,
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
                    Relaties = Array.Empty<Relatie>(),
                    Bron = Bron.KBO.Waarde,
                },
                Metadata = new Metadata
                {
                    DatumLaatsteAanpassing = "2020-05-15",
                },
            },
        };
    }
}
