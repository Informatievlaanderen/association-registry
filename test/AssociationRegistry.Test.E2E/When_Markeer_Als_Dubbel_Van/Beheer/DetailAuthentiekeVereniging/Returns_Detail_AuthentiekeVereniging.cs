namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van.Beheer.DetailAuthentiekeVereniging;

using Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using Admin.Schema.Constants;
using AssociationRegistry.Test.Common.DubbelMerge;
using FluentAssertions;
using Formats;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Xunit;
using ITestOutputHelper = Xunit.ITestOutputHelper;

[Collection(nameof(MarkeerAlsDubbelVanCollection))]
public class Returns_Vereniging : End2EndTest<DetailVerenigingResponse>
{
    private readonly MarkeerAlsDubbelVanContext _testContext;
    private readonly ITestOutputHelper _helper;

    public Returns_Vereniging(MarkeerAlsDubbelVanContext testContext, ITestOutputHelper helper)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
        _helper = helper;
    }

    public override async Task<DetailVerenigingResponse> GetResponse(FullBlownApiSetup setup) =>
        await setup.AdminApiHost.GetBeheerDetail(
            setup.AdminHttpClient,
            _testContext.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode,
            headers: new RequestParameters().WithExpectedSequence(
                _testContext.DubbeleVerenigingWerdSamengevoegd!.Sequence
            )
        );

    [Fact]
    public void With_IsDubbelVan_VCode_Of_AndereFeitelijkeVerenigingWerdGeregistreerd()
    {
        Response.Vereniging.IsDubbelVan.Should().BeEmpty();
    }

    [Fact]
    public async ValueTask With_DubbeleVereniging_In_CorresponderendeVCodes()
    {
        var tryCounter = 0;

        while (tryCounter < 20)
        {
            ++tryCounter;
            await Task.Delay(500);

            _helper.WriteLine($"Looking for CorresponderendeVCodes (try {tryCounter})...");
            if (Response.Vereniging.CorresponderendeVCodes.Any())
            {
                _helper.WriteLine("Found it!");
                break;
            }

            _helper.WriteLine("Did not find any CorresponderendeVCodes.");
        }
        Response
            .Vereniging.CorresponderendeVCodes.Should()
            .Contain(_testContext.Scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode);
    }

    [Fact]
    public void With_The_Own_KorteNaam_Kept() =>
        Response
            .Vereniging.KorteNaam.Should()
            .Be(MarkeerAlsDubbelVanContext.BasisgegevensAuthentiekeVereniging.KorteNaam);

    [Fact]
    public void With_The_KorteBeschrijving_Of_The_Dubbele_Vereniging() =>
        Response
            .Vereniging.KorteBeschrijving.Should()
            .Be(MarkeerAlsDubbelVanContext.BasisgegevensDubbeleVereniging.KorteBeschrijving);

    [Fact]
    public void With_The_Startdatum_Of_The_Dubbele_Vereniging() =>
        Response
            .Vereniging.Startdatum.Should()
            .Be(MarkeerAlsDubbelVanContext.BasisgegevensDubbeleVereniging.Startdatum!.Value.FormatAsBelgianDate());

    [Fact]
    public void With_The_Own_And_The_New_Hoofdactiviteiten() =>
        Response
            .Vereniging.HoofdactiviteitenVerenigingsloket.Select(x => x.Code)
            .Should()
            .BeEquivalentTo([TestHoofdactiviteiten.Sport.Code, TestHoofdactiviteiten.Jeugdwerk.Code]);

    [Fact]
    public void With_The_Werkingsgebieden_Of_The_Dubbele_Vereniging() =>
        Response
            .Vereniging.Werkingsgebieden.Select(x => x.Code)
            .Should()
            .BeEquivalentTo([TestWerkingsgebieden.VlaamsBrabant.Code]);

    [Fact]
    public void With_The_Own_And_The_Overgenomen_Contactgegevens() =>
        Response
            .Vereniging.Contactgegevens.Select(x => new
            {
                x.ContactgegevenId,
                x.Waarde,
                x.IsPrimair,
            })
            .Should()
            .BeEquivalentTo([
                new
                {
                    ContactgegevenId = 1,
                    TestContactgegevens.EmailRivendell.Waarde,
                    IsPrimair = true,
                },
                new
                {
                    ContactgegevenId = 2,
                    TestContactgegevens.TelefoonRivendell.Waarde,
                    IsPrimair = false,
                },
                new
                {
                    ContactgegevenId = 3,
                    TestContactgegevens.EmailFellowship.Waarde,
                    IsPrimair = false,
                },
                new
                {
                    ContactgegevenId = 4,
                    TestContactgegevens.WebsiteFellowship.Waarde,
                    IsPrimair = true,
                },
                new
                {
                    ContactgegevenId = 5,
                    TestContactgegevens.SocialMediaFellowship.Waarde,
                    IsPrimair = false,
                },
            ]);

    [Fact]
    public void With_The_Own_And_The_Overgenomen_Locaties() =>
        Response
            .Vereniging.Locaties.Select(x => new
            {
                x.Naam,
                x.Locatietype,
                x.IsPrimair,
            })
            .Should()
            .BeEquivalentTo([
                new
                {
                    TestLocaties.LokaalRivendell.Naam,
                    TestLocaties.LokaalRivendell.Locatietype,
                    IsPrimair = true,
                },
                new
                {
                    TestLocaties.PostadresRivendell.Naam,
                    TestLocaties.PostadresRivendell.Locatietype,
                    IsPrimair = false,
                },
                new
                {
                    TestLocaties.LokaalFellowship.Naam,
                    TestLocaties.LokaalFellowship.Locatietype,
                    IsPrimair = false,
                },
                new
                {
                    TestLocaties.TweedeLokaalFellowship.Naam,
                    TestLocaties.TweedeLokaalFellowship.Locatietype,
                    IsPrimair = false,
                },
            ]);

    [Fact]
    public void With_The_Own_And_The_Overgenomen_Vertegenwoordigers() =>
        Response
            .Vereniging.Vertegenwoordigers.Select(x => new
            {
                x.VertegenwoordigerId,
                x.Insz,
                x.Voornaam,
                x.Email,
                x.PrimairContactpersoon,
            })
            .Should()
            .BeEquivalentTo([
                new
                {
                    VertegenwoordigerId = 1,
                    TestVertegenwoordigers.Frodo.Insz,
                    TestVertegenwoordigers.Frodo.Voornaam,
                    TestVertegenwoordigers.Frodo.Email,
                    PrimairContactpersoon = true,
                },
                new
                {
                    VertegenwoordigerId = 2,
                    TestVertegenwoordigers.Aragorn.Insz,
                    TestVertegenwoordigers.Aragorn.Voornaam,
                    TestVertegenwoordigers.Aragorn.Email,
                    PrimairContactpersoon = false,
                },
                new
                {
                    VertegenwoordigerId = 3,
                    TestVertegenwoordigers.Samwise.Insz,
                    TestVertegenwoordigers.Samwise.Voornaam,
                    TestVertegenwoordigers.Samwise.Email,
                    PrimairContactpersoon = false,
                },
            ]);

    [Fact]
    public void With_The_Own_And_The_Overgenomen_Bankrekeningnummers() =>
        Response
            .Vereniging.Bankrekeningnummers.Select(x => new
            {
                x.BankrekeningnummerId,
                x.Iban,
                x.Doel,
                x.Titularissen,
                BevestigdDoor = x.BevestigdDoor.Select(b => b.OvoCode).ToArray(),
            })
            .Should()
            .BeEquivalentTo([
                new
                {
                    BankrekeningnummerId = 1,
                    TestBankrekeningnummers.BankrekeningnummerFrodo.Iban,
                    TestBankrekeningnummers.BankrekeningnummerFrodo.Doel,
                    TestBankrekeningnummers.BankrekeningnummerFrodo.Titularissen,
                    BevestigdDoor = TestBankrekeningnummers
                        .BankrekeningnummerFrodo.BevestigdDoor.Select(b => b.OvoCode)
                        .ToArray(),
                },
                new
                {
                    BankrekeningnummerId = 2,
                    TestBankrekeningnummers.BankrekeningnummerSamwise.Iban,
                    TestBankrekeningnummers.BankrekeningnummerSamwise.Doel,
                    TestBankrekeningnummers.BankrekeningnummerSamwise.Titularissen,
                    BevestigdDoor = TestBankrekeningnummers
                        .BankrekeningnummerSamwise.BevestigdDoor.Select(b => b.OvoCode)
                        .ToArray(),
                },
            ]);

    [Fact]
    public void With_The_Own_And_The_Overgenomen_Erkenningen() =>
        Response
            .Vereniging.Erkenningen.Select(x => new
            {
                x.ErkenningId,
                x.IpdcProduct.Nummer,
                x.GeregistreerdDoor.OvoCode,
                x.Startdatum,
                x.Einddatum,
                x.Hernieuwingsdatum,
                x.HernieuwingsUrl,
                x.Status,
            })
            .Should()
            .BeEquivalentTo([
                new
                {
                    ErkenningId = 1,
                    TestErkenningen.LembasBakken.Nummer,
                    TestErkenningen.Rivendell.OvoCode,
                    Startdatum = (string?)"2020-01-01",
                    Einddatum = (string?)"2021-12-31",
                    Hernieuwingsdatum = (string?)null,
                    HernieuwingsUrl = string.Empty,
                    TestErkenningen.LembasBakkenRivendell.Status,
                },
                new
                {
                    ErkenningId = 2,
                    TestErkenningen.RingdragersBegeleiden.Nummer,
                    TestErkenningen.Gondor.OvoCode,
                    Startdatum = (string?)"2024-01-01",
                    Einddatum = (string?)null,
                    Hernieuwingsdatum = (string?)"2030-01-01",
                    TestErkenningen.RingdragersBegeleidenGondor.HernieuwingsUrl,
                    TestErkenningen.RingdragersBegeleidenGondor.Status,
                },
            ]);

    [Fact]
    public void With_The_Doelgroep_Of_The_Dubbele_Vereniging()
    {
        var doelgroep = MarkeerAlsDubbelVanContext.BasisgegevensDubbeleVereniging.Doelgroep;

        Response.Vereniging.Doelgroep.Minimumleeftijd.Should().Be(doelgroep.Minimumleeftijd);
        Response.Vereniging.Doelgroep.Maximumleeftijd.Should().Be(doelgroep.Maximumleeftijd);
    }

    [Fact]
    public void With_Status_Is_Actief()
    {
        Response.Vereniging.Status.Should().Be(VerenigingStatus.Actief);
    }
}
