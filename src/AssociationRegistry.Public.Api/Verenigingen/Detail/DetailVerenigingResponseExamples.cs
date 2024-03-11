﻿namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

using Infrastructure.ConfigurationBindings;
using JsonLdContext;
using ResponseModels;
using Schema.Constants;
using Swashbuckle.AspNetCore.Filters;
using System;
using Vereniging;
using Adres = ResponseModels.Adres;
using AdresId = ResponseModels.AdresId;
using Contactgegeven = ResponseModels.Contactgegeven;
using HoofdactiviteitVerenigingsloket = ResponseModels.HoofdactiviteitVerenigingsloket;
using Locatie = ResponseModels.Locatie;
using Vereniging = ResponseModels.Vereniging;

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
            Context = $"{_appSettings.BaseUrl}/v1/contexten/publiek/detail-vereniging-context.json",
            Vereniging = new Vereniging
            {
                type = JsonLdType.FeitelijkeVereniging.Type,
                VCode = "V0001001",
                Verenigingstype = new VerenigingsType
                {
                    Code = "FV",
                    Naam = "Feitelijke vereniging",
                },
                Naam = "FWA De vrolijke BA’s",
                KorteNaam = "DVB",
                KorteBeschrijving = "De vereniging van de vrolijke BA's",
                Startdatum = new DateOnly(year: 2020, month: 05, day: 15),
                Doelgroep = new DoelgroepResponse
                {
                    id = JsonLdType.Doelgroep.CreateWithIdValues("V0001001"),
                    type = JsonLdType.Doelgroep.Type,
                    Minimumleeftijd = 0,
                    Maximumleeftijd = 150,
                },
                Status = VerenigingStatus.Actief,
                Contactgegevens = new[]
                {
                    new Contactgegeven
                    {
                        id = JsonLdType.Contactgegeven.CreateWithIdValues("V0001001", "1"),
                        type = JsonLdType.Contactgegeven.Type,
                        Contactgegeventype = "E-mail",
                        Waarde = "info@example.org",
                        Beschrijving = "Info",
                        IsPrimair = false,
                    },
                },
                Locaties = new[]
                {
                    new Locatie
                    {
                        id = JsonLdType.Locatie.CreateWithIdValues("V0001001", "1"),
                        type = JsonLdType.Locatie.Type,

                        Locatietype = Locatietype.Correspondentie.Waarde,
                        IsPrimair = true,
                        Adresvoorstelling = "kerkstraat 5, 1770 Liedekerke, Belgie",
                        Naam = "de kerk",
                        Adres = new Adres
                        {
                            id = JsonLdType.Adres.CreateWithIdValues("V0001001", "1"),
                            type = JsonLdType.Adres.Type,
                            Straatnaam = "kerkstraat",
                            Huisnummer = "5",
                            Busnummer = null,
                            Postcode = "1770",
                            Gemeente = "Liedekerke",
                            Land = "Belgie",
                        },
                        AdresId = new AdresId
                        {
                            Broncode = Adresbron.AR.Code,
                            Bronwaarde = AssociationRegistry.Vereniging.AdresId.DataVlaanderenAdresPrefix + "1",
                        },
                        VerwijstNaar = new AdresVerwijzing
                        {
                            id = JsonLdType.AdresVerwijzing.CreateWithIdValues("1"),
                            type = JsonLdType.AdresVerwijzing.Type,
                        },
                    },
                },
                HoofdactiviteitenVerenigingsloket = new[]
                {
                    new HoofdactiviteitVerenigingsloket
                    {
                        id = JsonLdType.Hoofdactiviteit.CreateWithIdValues("CULT"),
                        type = JsonLdType.Hoofdactiviteit.Type,

                        Code = "CULT",
                        Naam = "Cultuur",
                    },
                },
                Sleutels = new []
                {
                    new Sleutel
                    {
                        id = JsonLdType.Sleutel.CreateWithIdValues("V0001001", Sleutelbron.VR),
                        type = JsonLdType.Sleutel.Type,
                        Bron = Sleutelbron.VR,
                        Waarde = "V0001001",
                        CodeerSysteem = CodeerSysteem.VR,
                        GestructureerdeIdentificator = new GestructureerdeIdentificator
                        {
                            id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues("V0001001", Sleutelbron.VR),
                            type = JsonLdType.GestructureerdeSleutel.Type,
                            Nummer = "V0001001",
                        },
                    },
                },
                Relaties = Array.Empty<Relatie>(),
            },
            Metadata = new Metadata { DatumLaatsteAanpassing = "2023-05-15" },
        };
}
