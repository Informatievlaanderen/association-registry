namespace AssociationRegistry.Public.Api.WebApi.Verenigingen.Detail.ResponseExamples;

using AssociationRegistry.Public.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Vereniging;
using Contracts.JsonLdContext;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Adressen;
using DecentraalBeheer.Vereniging.Mappers;
using ResponseModels;
using Swashbuckle.AspNetCore.Filters;
using Adres = ResponseModels.Adres;
using AdresId = ResponseModels.AdresId;
using Contactgegeven = ResponseModels.Contactgegeven;
using HoofdactiviteitVerenigingsloket = ResponseModels.HoofdactiviteitVerenigingsloket;
using Lidmaatschap = ResponseModels.Lidmaatschap;
using Locatie = ResponseModels.Locatie;
using Vereniging = ResponseModels.Vereniging;
using Verenigingssubtype = ResponseModels.Verenigingssubtype;
using VerenigingStatus = Schema.Constants.VerenigingStatus;
using Verenigingstype = ResponseModels.Verenigingstype;
using Werkingsgebied = ResponseModels.Werkingsgebied;

public class DetailVerenigingResponseExamples : IMultipleExamplesProvider<PubliekVerenigingDetailResponse>
{
    private readonly AppSettings _appSettings;

    public DetailVerenigingResponseExamples(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public IEnumerable<SwaggerExample<PubliekVerenigingDetailResponse>> GetExamples()
    {
        yield return SwaggerExample.Create(
            "Zonder versie header",
            DetailResponse(
                VrolijkeBas(type: new Verenigingstype { Code = "FV", Naam = "Feitelijke vereniging" }, subtype: null)
            )
        );

        yield return SwaggerExample.Create(
            name: "Versie 'v2'",
            DetailResponse(
                VrolijkeBas(
                    type: new Verenigingstype { Code = "VZER", Naam = "Vereniging zonder eigen rechtspersoonlijkheid" },
                    subtype: VerenigingssubtypeCode.NietBepaald.Map<Verenigingssubtype>()
                )
            )
        );
    }

    private PubliekVerenigingDetailResponse DetailResponse(Vereniging vereniging) =>
        new()
        {
            Context = $"{_appSettings.BaseUrl}/v1/contexten/publiek/detail-vereniging-context.json",
            Vereniging = vereniging,
            Metadata = new Metadata { DatumLaatsteAanpassing = "2023-05-15" },
        };

    private static Vereniging VrolijkeBas(Verenigingstype type, Verenigingssubtype? subtype) =>
        new()
        {
            type = JsonLdType.FeitelijkeVereniging.Type,
            VCode = "V0001001",
            Verenigingstype = type,
            Verenigingssubtype = subtype,
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
            Contactgegevens = new[] { InfoEmail() },
            Locaties = new[] { KerkLiedekerke() },
            HoofdactiviteitenVerenigingsloket = new[] { HoofdactiviteitCult() },
            Werkingsgebieden = new[] { WerkingsgebiedWestVlaanderen() },
            Sleutels = new[] { VrSleutel() },
            Relaties = Array.Empty<Relatie>(),
            Lidmaatschappen = new[] { LidmaatschapVasteDatums() },
        };

    private static Contactgegeven InfoEmail() =>
        new()
        {
            id = JsonLdType.Contactgegeven.CreateWithIdValues("V0001001", "1"),
            type = JsonLdType.Contactgegeven.Type,
            Contactgegeventype = "E-mail",
            Waarde = "info@example.org",
            Beschrijving = "Info",
            IsPrimair = false,
        };

    private static Locatie KerkLiedekerke() =>
        new()
        {
            id = JsonLdType.Locatie.CreateWithIdValues("V0001001", "1"),
            type = JsonLdType.Locatie.Type,
            Locatietype = Locatietype.Correspondentie.Waarde,
            IsPrimair = true,
            Adresvoorstelling = "kerkstraat 5, 1770 Liedekerke, België",
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
                Land = "België",
            },
            AdresId = new AdresId
            {
                Broncode = Adresbron.AR.Code,
                Bronwaarde = DecentraalBeheer.Vereniging.Adressen.AdresId.DataVlaanderenAdresPrefix + "1",
            },
            VerwijstNaar = new AdresVerwijzing
            {
                id = JsonLdType.AdresVerwijzing.CreateWithIdValues("1"),
                type = JsonLdType.AdresVerwijzing.Type,
            },
        };

    private static HoofdactiviteitVerenigingsloket HoofdactiviteitCult() =>
        new()
        {
            id = JsonLdType.Hoofdactiviteit.CreateWithIdValues("CULT"),
            type = JsonLdType.Hoofdactiviteit.Type,
            Code = "CULT",
            Naam = "Cultuur",
        };

    private static Werkingsgebied WerkingsgebiedWestVlaanderen() =>
        new()
        {
            id = JsonLdType.Werkingsgebied.CreateWithIdValues("CULT"),
            type = JsonLdType.Werkingsgebied.Type,
            Code = "BE25",
            Naam = "Provincie West-Vlaanderen",
        };

    private static Sleutel VrSleutel() =>
        new()
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
        };

    private static Lidmaatschap LidmaatschapVasteDatums() =>
        new()
        {
            id = JsonLdType.Lidmaatschap.CreateWithIdValues("V0001001", "1"),
            type = JsonLdType.Lidmaatschap.Type,
            Beschrijving = "Een lidmaatschap",
            Naam = "De andere vereniging",
            Van = "2002-11-15",
            Tot = "2002-11-16",
            Identificatie = "Een identificatie",
            AndereVereniging = "V0001111",
        };
}
