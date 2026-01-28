namespace AssociationRegistry.Test.E2E.When_Registreer_VerenigingMetRechtspersoonlijkheid.Beheer.Detail.With_Header;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Contracts.JsonLdContext;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Formats;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Newtonsoft.Json;
using NodaTime;
using Xunit;
using Locatie = Admin.Api.WebApi.Verenigingen.Detail.ResponseModels.Locatie;
using Verenigingstype = Admin.Api.WebApi.Verenigingen.Detail.ResponseModels.Verenigingstype;
using Vertegenwoordiger = Admin.Api.WebApi.Verenigingen.Detail.ResponseModels.Vertegenwoordiger;

/// <summary>
/// <see cref="wiremock/__files/GeefOndernemingResponses/fallback.xml"/>
/// </summary>
[Collection(nameof(RegistreerVerenigingMetRechtsperoonlijkheidCollection))]
public class Returns_Vereniging : End2EndTest<DetailVerenigingResponse>
{
    private readonly RegistreerVerenigingMetRechtsperoonlijkheidContext _testContext;

    public Returns_Vereniging(RegistreerVerenigingMetRechtsperoonlijkheidContext testContext) : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup)
        => await setup.AdminApiHost
                      .GetBeheerDetail(setup.AdminHttpClient, _testContext.VCode, headers:
                                       new RequestParameters().V2().WithExpectedSequence(_testContext.CommandResult.Sequence));

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/detail-vereniging-context.json");
    }

    [Fact]
    public void With_Metadata_DatumLaatsteAanpassing()
    {
        Response.Metadata.DatumLaatsteAanpassing.ShouldCompare(Instant.FromDateTimeOffset(DateTimeOffset.Now).FormatAsBelgianDate(),
                                                               compareConfig: new ComparisonConfig
                                                                   { MaxMillisecondsDateDifference = 5000 });
    }

    [Fact]
    public async ValueTask With_VereningMetRechtspersoonlijkheid()
    {
        var p = JsonConvert.SerializeObject(Response.Vereniging);

        Response.Vereniging.Should().BeEquivalentTo(new VerenigingDetail
        {
            type = JsonLdType.VerenigingMetRechtspersoonlijkheid.Type,
            CorresponderendeVCodes = [],
            Doelgroep = new DoelgroepResponse
            {
                type = JsonLdType.Doelgroep.Type,
                id = JsonLdType.Doelgroep.CreateWithIdValues(_testContext.VCode),
                Minimumleeftijd = 0,
                Maximumleeftijd = 150,
            },
            VCode = _testContext.VCode,
            KorteBeschrijving = string.Empty,
            KorteNaam = "V.L.K.",
            Roepnaam = string.Empty,
            Verenigingstype = new Verenigingstype
            {
                Code = DecentraalBeheer.Vereniging.Verenigingstype.VZW.Code,
                Naam = DecentraalBeheer.Vereniging.Verenigingstype.VZW.Naam,
            },
            Naam = "0451289431",
            Einddatum = null,
            Status = VerenigingStatus.Actief.StatusNaam,
            IsUitgeschrevenUitPubliekeDatastroom = false,
            Contactgegevens = [],
            HoofdactiviteitenVerenigingsloket = [],
            Werkingsgebieden = [],
            Bron = "KBO",
            Startdatum = Instant
                        .FromDateTimeOffset(new DateTimeOffset(1989, 10, 03, 0, 0, 0, TimeSpan.Zero))
                        .FormatAsBelgianDate(),
            Locaties =
            [
                new Locatie
                {
                    id = JsonLdType.Locatie.CreateWithIdValues(_testContext.VCode, "1"),
                                                               type = "org:Site",
                                                               LocatieId = 1,
                                                               Locatietype = "Maatschappelijke zetel volgens KBO",
                                                               IsPrimair = false,
                                                               Naam = "",
                                                               Adres = new Adres
                                                               {
                                                                   id = JsonLdType.Adres.CreateWithIdValues(_testContext.VCode, "1"),
                                                                   type = "locn:Address",
                                                                   Straatnaam = "Koningsstraat",
                                                                   Huisnummer = "217",
                                                                   Busnummer = "",
                                                                   Postcode = "1210",
                                                                   Gemeente = "Sint-Joost-ten-Node",
                                                                   Land = "België",
                                                               },
                                                               Adresvoorstelling = "Koningsstraat 217, 1210 Sint-Joost-ten-Node, België",
                                                               AdresId = null,
                                                               VerwijstNaar = null,
                                                               Bron = "KBO",
                },
            ],

            Vertegenwoordigers =
            [
                new Vertegenwoordiger
                {
                    id = JsonLdType.Vertegenwoordiger.CreateWithIdValues(_testContext.VCode, "1"),
                    type = "person:Person",
                    VertegenwoordigerId = 1,
                    Insz = "1234567890",

                    Voornaam = "Frodo",
                    Achternaam = "Baggins",

                    Roepnaam = "",
                    Rol = "",
                    Email = "",
                    Telefoon = "",
                    Mobiel = "",
                    SocialMedia = "",
                    VertegenwoordigerContactgegevens = new VertegenwoordigerContactgegevens()
                    {
                        id = JsonLdType.VertegenwoordigerContactgegeven.CreateWithIdValues(_testContext.VCode, "1"),
                        type = "schema:ContactPoint",
                        IsPrimair = false,
                        Email = "",
                        Telefoon = "",
                        Mobiel = "",
                        SocialMedia = "",
                    },
                    Bron = "KBO",
                },
                new Vertegenwoordiger
                {
                    id = JsonLdType.Vertegenwoordiger.CreateWithIdValues(_testContext.VCode, "2"),
                    type = "person:Person",
                    VertegenwoordigerId = 2,
                    Insz = "0987654321",
                    Voornaam = "Sam",
                    Achternaam = "Gamgee",
                    Roepnaam = "",
                    Rol = "",
                    Email = "",
                    Telefoon = "",
                    Mobiel = "",
                    SocialMedia = "",
                    VertegenwoordigerContactgegevens = new VertegenwoordigerContactgegevens()
                    {
                        id = JsonLdType.VertegenwoordigerContactgegeven.CreateWithIdValues(_testContext.VCode, "2"),
                        type = "schema:ContactPoint",
                        IsPrimair = false,
                        Email = "",
                        Telefoon = "",
                        Mobiel = "",
                        SocialMedia = "",
                    },
                    Bron = "KBO",
                },
            ],

            Sleutels =
            [
                new Sleutel
                {
                    id = JsonLdType.Sleutel.CreateWithIdValues(_testContext.VCode, Sleutelbron.VR.Waarde),
                    type = "adms:Identifier",
                    Bron = "Vcode",
                    Waarde = _testContext.VCode,
                    CodeerSysteem = "Vcode",
                    GestructureerdeIdentificator = new GestructureerdeIdentificator
                    {
                        id =  JsonLdType.GestructureerdeSleutel.CreateWithIdValues(_testContext.VCode, Sleutelbron.VR.Waarde),
                        type = "generiek:GestructureerdeIdentificator",
                        Nummer = _testContext.VCode,
                    },
                },
                new Sleutel
                {
                    id = JsonLdType.Sleutel.CreateWithIdValues(_testContext.VCode, Sleutelbron.KBO.Waarde),
                    type = "adms:Identifier",
                    Bron = "KBO",
                    Waarde = "0451289431",
                    CodeerSysteem = "KBO nummer",
                    GestructureerdeIdentificator = new GestructureerdeIdentificator
                    {
                        id = JsonLdType.GestructureerdeSleutel.CreateWithIdValues(_testContext.VCode, Sleutelbron.KBO.Waarde),
                        type = "generiek:GestructureerdeIdentificator",
                        Nummer = "0451289431",
                    },
                },
            ],
            Relaties = [],
            Lidmaatschappen = [],
            IsDubbelVan = string.Empty,
            Bankrekeningnummers = [],
        });
    }
}
