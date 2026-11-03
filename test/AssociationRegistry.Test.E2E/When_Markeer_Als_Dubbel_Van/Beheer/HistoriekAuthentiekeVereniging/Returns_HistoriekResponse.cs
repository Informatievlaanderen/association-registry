namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van.Beheer.HistoriekAuthentiekeVereniging;

using Admin.Api.WebApi.Verenigingen.Historiek.ResponseModels;
using AssociationRegistry.Test.Common.DubbelMerge;
using Events;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.Comparison;
using Framework.Mappers;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using Xunit;
using ITestOutputHelper = Xunit.ITestOutputHelper;

[Collection(nameof(MarkeerAlsDubbelVanCollection))]
public class Returns_Vereniging : End2EndTest<HistoriekResponse>
{
    private readonly MarkeerAlsDubbelVanContext _testContext;
    private readonly ITestOutputHelper _helper;

    public Returns_Vereniging(MarkeerAlsDubbelVanContext testContext, ITestOutputHelper helper)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
        _helper = helper;
    }

    public override async Task<HistoriekResponse> GetResponse(FullBlownApiSetup setup) =>
        await setup.AdminApiHost.GetBeheerHistoriek(
            setup.AdminHttpClient,
            _testContext.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode,
            headers: new RequestParameters().WithExpectedSequence(
                _testContext.DubbeleVerenigingWerdSamengevoegd!.Sequence
            )
        );

    [Fact]
    public async ValueTask With_VCode()
    {
        var tryCounter = 0;

        while (tryCounter < 20)
        {
            ++tryCounter;
            await Task.Delay(500);

            _helper.WriteLine($"Looking for CorresponderendeVCodes (try {tryCounter})...");
            if (Response.VCode is not null)
            {
                _helper.WriteLine("Found it!");
                break;
            }

            _helper.WriteLine("Did not find any CorresponderendeVCodes.");
        }

        Response.VCode.ShouldCompare(_testContext.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode);
    }

    [Fact]
    public void With_Context()
    {
        Response.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/historiek-vereniging-context.json");
    }

#pragma warning disable CS0618
    [Fact]
    public void With_The_Samenvoeging_After_The_Aanvaarding()
    {
        var gebeurtenissen = Response.Gebeurtenissen.Select(x => x.Gebeurtenis).ToArray();

        gebeurtenissen
            .Should()
            .EndWith([
                nameof(VerenigingAanvaarddeDubbeleVereniging),
                nameof(DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging),
            ]);
    }

    /// <summary>
    ///     De authentieke vereniging had enkel een korte naam: die blijft behouden, de rest wordt overgenomen.
    ///     Jeugdwerk en Vlaams-Brabant worden toegevoegd, Sport had de authentieke vereniging al.
    /// </summary>
    [Fact]
    public void With_The_Samenvoeging_Gebeurtenis()
    {
        var dubbele = MarkeerAlsDubbelVanContext.BasisgegevensDubbeleVereniging;

        var gebeurtenis = Response.Gebeurtenissen.Single(x =>
            x.Gebeurtenis == nameof(DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging)
        );

        var overgenomen = SamenvoegingDubbeleVereniging.OvergenomenGegevensPerGroep<
            Registratiedata.Vertegenwoordiger,
            SamenvoegingDubbeleVereniging.Bankrekeningnummer
        >.WithOnly(
            new SamenvoegingDubbeleVereniging.OvergenomenBasisgegevens(
                KorteNaam: null,
                KorteBeschrijving: SamenvoegingDubbeleVereniging.Verrijkt(
                    SamenvoegingDubbeleVereniging.Velden.KorteBeschrijving,
                    dubbele.KorteBeschrijving
                ),
                Startdatum: SamenvoegingDubbeleVereniging.Verrijkt(
                    SamenvoegingDubbeleVereniging.Velden.Startdatum,
                    dubbele.Startdatum!.Value
                ),
                Doelgroep: SamenvoegingDubbeleVereniging.Verrijkt(
                    SamenvoegingDubbeleVereniging.Velden.Doelgroep,
                    dubbele.Doelgroep
                )
            )
        ) with
        {
            Hoofdactiviteiten =
            [
                SamenvoegingDubbeleVereniging.Toegevoegd(
                    "Hoofdactiviteit 'Jeugdwerk'",
                    TestHoofdactiviteiten.Jeugdwerk
                ),
            ],
            Werkingsgebieden =
            [
                SamenvoegingDubbeleVereniging.Toegevoegd(
                    "Werkingsgebied 'Provincie Vlaams-Brabant'",
                    TestWerkingsgebieden.VlaamsBrabant
                ),
            ],
            Contactgegevens =
            [
                SamenvoegingDubbeleVereniging.ToegevoegdMetAanpassing(
                    "E-mail 'info@fellowship.example' werd toegevoegd als niet-primair, omdat de authentieke vereniging al een primair contactgegeven van dit type heeft.",
                    TestContactgegevens.EmailFellowship,
                    TestContactgegevens.EmailFellowship with
                    {
                        ContactgegevenId = 3,
                        IsPrimair = false,
                    }
                ),
                new SamenvoegingDubbeleVereniging.OvergenomenGegeven<Registratiedata.Contactgegeven>(
                    SamenvoegingDubbeleVereniging.Acties.Toegevoegd,
                    "Website 'https://fellowship.example' werd toegevoegd vanuit de dubbele vereniging.",
                    TestContactgegevens.WebsiteFellowship,
                    TestContactgegevens.WebsiteFellowship with
                    {
                        ContactgegevenId = 4,
                    }
                ),
                new SamenvoegingDubbeleVereniging.OvergenomenGegeven<Registratiedata.Contactgegeven>(
                    SamenvoegingDubbeleVereniging.Acties.Toegevoegd,
                    "SocialMedia 'https://social.example/fellowship' werd toegevoegd vanuit de dubbele vereniging.",
                    TestContactgegevens.SocialMediaFellowship,
                    TestContactgegevens.SocialMediaFellowship with
                    {
                        ContactgegevenId = 5,
                    }
                ),
            ],
            Locaties =
            [
                SamenvoegingDubbeleVereniging.ToegevoegdMetAanpassing(
                    "Locatie 'Bag End' werd toegevoegd als niet-primair, omdat de authentieke vereniging al een primaire locatie heeft.",
                    TestLocaties.LokaalFellowship,
                    TestLocaties.LokaalFellowship with
                    {
                        LocatieId = 3,
                        IsPrimair = false,
                    }
                ),
                new SamenvoegingDubbeleVereniging.OvergenomenGegeven<Registratiedata.Locatie>(
                    SamenvoegingDubbeleVereniging.Acties.Toegevoegd,
                    "Locatie 'The Prancing Pony' werd toegevoegd vanuit de dubbele vereniging.",
                    TestLocaties.TweedeLokaalFellowship,
                    TestLocaties.TweedeLokaalFellowship with
                    {
                        LocatieId = 4,
                    }
                ),
            ],
            Vertegenwoordigers =
            [
                SamenvoegingDubbeleVereniging.ToegevoegdMetAanpassing(
                    "Vertegenwoordiger met id 2 werd toegevoegd als niet-primair, omdat de authentieke vereniging al een primaire vertegenwoordiger heeft.",
                    MarkeerAlsDubbelVanContext.AragornAlsPrimair,
                    TestVertegenwoordigers.Aragorn with
                    {
                        VertegenwoordigerId = 2,
                        IsPrimair = false,
                    }
                ),
                new SamenvoegingDubbeleVereniging.OvergenomenGegeven<Registratiedata.Vertegenwoordiger>(
                    SamenvoegingDubbeleVereniging.Acties.Toegevoegd,
                    "Vertegenwoordiger met id 3 werd toegevoegd vanuit de dubbele vereniging.",
                    TestVertegenwoordigers.Samwise,
                    TestVertegenwoordigers.Samwise with
                    {
                        VertegenwoordigerId = 3,
                    }
                ),
            ],
            Bankrekeningnummers =
            [
                new SamenvoegingDubbeleVereniging.OvergenomenGegeven<SamenvoegingDubbeleVereniging.Bankrekeningnummer>(
                    SamenvoegingDubbeleVereniging.Acties.Toegevoegd,
                    "Bankrekeningnummer met id 4 werd toegevoegd vanuit de dubbele vereniging.",
                    TestBankrekeningnummers.BankrekeningnummerSamwise,
                    TestBankrekeningnummers.BankrekeningnummerSamwise with
                    {
                        BankrekeningnummerId = 2,
                    }
                ),
            ],
            Erkenningen =
            [
                new SamenvoegingDubbeleVereniging.OvergenomenGegeven<SamenvoegingDubbeleVereniging.Erkenning>(
                    SamenvoegingDubbeleVereniging.Acties.Toegevoegd,
                    "Erkenning 'Ringdragers begeleiden' werd toegevoegd vanuit de dubbele vereniging.",
                    TestErkenningen.RingdragersBegeleidenGondor.ToSamenvoeging(),
                    TestErkenningen.RingdragersBegeleidenGondor.ToSamenvoeging() with
                    {
                        ErkenningId = 2,
                    }
                ),
            ],
        };

        var nietOvergenomen = SamenvoegingDubbeleVereniging.NietOvergenomenGegevensPerGroep<
            Registratiedata.Vertegenwoordiger,
            SamenvoegingDubbeleVereniging.Bankrekeningnummer
        >.WithOnly(
            SamenvoegingDubbeleVereniging.NietOvergenomenBasisgegevens.Geen with
            {
                KorteNaam = SamenvoegingDubbeleVereniging.NietOvergenomen(
                    SamenvoegingDubbeleVereniging.Velden.KorteNaam,
                    dubbele.KorteNaam
                ),
            }
        ) with
        {
            Hoofdactiviteiten =
            [
                SamenvoegingDubbeleVereniging.Gededupliceerd("Hoofdactiviteit 'Sport'", TestHoofdactiviteiten.Sport),
            ],
            Contactgegevens =
            [
                SamenvoegingDubbeleVereniging.Gededupliceerd(
                    "Telefoon '+32 11 22 33 44'",
                    TestContactgegevens.TelefoonFellowship
                ),
                SamenvoegingDubbeleVereniging.NietOvergenomen(
                    "Telefoon 'geen nummer'",
                    TestContactgegevens.OngeldigeTelefoonFellowship,
                    AssociationRegistry
                        .DecentraalBeheer
                        .Vereniging
                        .Samenvoeging
                        .ContactgegevensSamenvoeging
                        .RedenNietValide
                ),
            ],
            Locaties =
            [
                SamenvoegingDubbeleVereniging.Gededupliceerd(
                    "Locatie 'Rivendell'",
                    TestLocaties.RivendellBijFellowship
                ),
                SamenvoegingDubbeleVereniging.NietOvergenomen(
                    "Locatie 'Postadres'",
                    TestLocaties.PostadresFellowship,
                    AssociationRegistry
                        .DecentraalBeheer
                        .Vereniging
                        .Samenvoeging
                        .LocatiesSamenvoeging
                        .RedenCorrespondentie
                ),
                SamenvoegingDubbeleVereniging.NietOvergenomen(
                    "Locatie 'Mordor'",
                    TestLocaties.OnvolledigeLocatieFellowship,
                    AssociationRegistry.DecentraalBeheer.Vereniging.Samenvoeging.LocatiesSamenvoeging.RedenNietValide
                ),
            ],
            Vertegenwoordigers =
            [
                SamenvoegingDubbeleVereniging.Gededupliceerd(
                    "Vertegenwoordiger met id 1",
                    MarkeerAlsDubbelVanContext.FrodoBijDeDubbeleVereniging
                ),
            ],
            Bankrekeningnummers =
            [
                SamenvoegingDubbeleVereniging.Gededupliceerd(
                    "Bankrekeningnummer met id 1",
                    MarkeerAlsDubbelVanContext.ZelfdeIbanBijDeDubbeleVereniging
                ),
            ],
            Erkenningen =
            [
                SamenvoegingDubbeleVereniging.Gededupliceerd(
                    "Erkenning 'Lembas bakken'",
                    TestErkenningen.LembasBakkenRivendell.ToSamenvoeging()
                ),
                SamenvoegingDubbeleVereniging.NietOvergenomen(
                    "Erkenning 'Lembas bakken'",
                    TestErkenningen.OverlappendLembasBakkenRivendell.ToSamenvoeging(),
                    AssociationRegistry.DecentraalBeheer.Vereniging.Samenvoeging.ErkenningenSamenvoeging.RedenOverlap
                ),
            ],
        };

        gebeurtenis.ShouldCompare(
            HistoriekGebeurtenisMapper.DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging(
                _testContext.Scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode,
                _testContext.Scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode,
                overgenomen,
                nietOvergenomen
            ),
            compareConfig: HistoriekComparisonConfig.Instance
        );
    }
#pragma warning restore CS0618

    [Fact]
    public async ValueTask With_All_Gebeurtenissen()
    {
        var gebeurtenis = Response.Gebeurtenissen.SingleOrDefault(x =>
            x.Gebeurtenis == nameof(VerenigingAanvaarddeDubbeleVereniging)
        );

        var tryCounter = 0;

        while (tryCounter < 20)
        {
            ++tryCounter;
            await Task.Delay(500);

            _helper.WriteLine($"Looking for CorresponderendeVCodes (try {tryCounter})...");
            if (gebeurtenis is not null)
            {
                _helper.WriteLine("Found it!");
                break;
            }

            _helper.WriteLine("Did not find any CorresponderendeVCodes.");
        }

        gebeurtenis.ShouldCompare(
            HistoriekGebeurtenisMapper.VerenigingAanvaarddeDubbeleVereniging(
                _testContext.CommandRequest,
                _testContext.VCode
            ),
            compareConfig: HistoriekComparisonConfig.Instance
        );
    }
}
