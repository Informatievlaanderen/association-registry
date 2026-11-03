namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van;

using Admin.Api.WebApi.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.MarkeerAlsDubbelVan.RequestModels;
using AssociationRegistry.Test.Common.DubbelMerge;
using Events;
using FluentAssertions;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests.FeitelijkeVereniging;
using Xunit;
using IEvent = JasperFx.Events.IEvent;

// CollectionFixture for database setup ==> Context
[CollectionDefinition(nameof(MarkeerAlsDubbelVanCollection))]
public class MarkeerAlsDubbelVanCollection : ICollectionFixture<MarkeerAlsDubbelVanContext>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

public class MarkeerAlsDubbelVanContext : TestContextBase<MultipleWerdGeregistreerdScenario, MarkeerAlsDubbelVanRequest>
{
    public IEvent? VerenigingAanvaarddeDubbeleVereniging { get; private set; }
    public IEvent? DubbeleVerenigingWerdSamengevoegd { get; private set; }
    public IEvent? VerenigingWerdErkend { get; private set; }

    /// <summary>De dubbele vereniging heeft alle basisgegevens, de authentieke enkel een korte naam.</summary>
    public static readonly TestBasisgegevens BasisgegevensDubbeleVereniging = TestBasisgegevens.Fellowship;

    public static readonly TestBasisgegevens BasisgegevensAuthentiekeVereniging = TestBasisgegevens.Empty with
    {
        KorteNaam = TestBasisgegevens.Rivendell.KorteNaam,
    };

    public static readonly Registratiedata.Vertegenwoordiger FrodoBijDeDubbeleVereniging =
        TestVertegenwoordigers.Frodo with
        {
            Email = "frodo@shire.example",
            Roepnaam = "Mr. Baggins",
        };

    public static readonly SamenvoegingDubbeleVereniging.Bankrekeningnummer ZelfdeIbanBijDeDubbeleVereniging =
        TestBankrekeningnummers.BankrekeningnummerFrodo with
        {
            Iban = "BE68 5390 0754 7034",
            Doel = "Lidgelden",
            Titularissen = ["Samwise Gamgee"],
            BevestigdDoor = [],
        };

    public static readonly Registratiedata.Vertegenwoordiger AragornAlsPrimair = TestVertegenwoordigers.Aragorn with
    {
        IsPrimair = true,
    };

    /// <summary>
    ///     Jeugdwerk en Vlaams-Brabant zijn nieuw voor de authentieke vereniging, Sport had ze al.
    ///     Van de contactgegevens wordt de e-mail niet-primair (de authentieke heeft al een primaire e-mail), blijft de
    ///     website primair, wordt de social media link toegevoegd, is het telefoonnummer hetzelfde en is het ongeldige
    ///     telefoonnummer niet overgenomen.
    ///     Van de locaties wordt Bag End niet-primair (de authentieke heeft al een primaire locatie), wordt The Prancing
    ///     Pony toegevoegd, is Rivendell hetzelfde adres, is het tweede correspondentieadres en de onvolledige locatie
    ///     niet overgenomen.
    ///     Van de vertegenwoordigers is Frodo dezelfde persoon (zelfde INSZ), wordt Aragorn niet-primair (de authentieke
    ///     heeft al een primaire vertegenwoordiger) en wordt Samwise toegevoegd.
    ///     Van de bankrekeningnummers is dat van Frodo hetzelfde IBAN en wordt dat van Samwise toegevoegd, met zijn
    ///     bevestiging.
    ///     Van de erkenningen is die voor Lembas bakken dezelfde, overlapt de tweede voor Lembas bakken (niet
    ///     overgenomen) en wordt de actieve erkenning voor Ringdragers begeleiden toegevoegd, waardoor de authentieke
    ///     vereniging (die enkel een verlopen erkenning had) erkend wordt.
    /// </summary>
    public static readonly TestVerenigingsgegevens GegevensDubbeleVereniging = new(
        BasisgegevensDubbeleVereniging,
        [TestHoofdactiviteiten.Sport, TestHoofdactiviteiten.Jeugdwerk],
        [TestWerkingsgebieden.VlaamsBrabant],
        [
            TestContactgegevens.EmailFellowship,
            TestContactgegevens.WebsiteFellowship,
            TestContactgegevens.TelefoonFellowship,
            TestContactgegevens.OngeldigeTelefoonFellowship,
            TestContactgegevens.SocialMediaFellowship,
        ],
        [
            TestLocaties.LokaalFellowship,
            TestLocaties.RivendellBijFellowship,
            TestLocaties.PostadresFellowship,
            TestLocaties.OnvolledigeLocatieFellowship,
            TestLocaties.TweedeLokaalFellowship,
        ],
        [FrodoBijDeDubbeleVereniging, AragornAlsPrimair, TestVertegenwoordigers.Samwise],
        [ZelfdeIbanBijDeDubbeleVereniging, TestBankrekeningnummers.BankrekeningnummerSamwise],
        [
            TestErkenningen.LembasBakkenRivendell,
            TestErkenningen.OverlappendLembasBakkenRivendell,
            TestErkenningen.RingdragersBegeleidenGondor,
        ]
    );

    public static readonly TestVerenigingsgegevens GegevensAuthentiekeVereniging = new(
        BasisgegevensAuthentiekeVereniging,
        [TestHoofdactiviteiten.Sport],
        [],
        [TestContactgegevens.EmailRivendell, TestContactgegevens.TelefoonRivendell],
        [TestLocaties.LokaalRivendell, TestLocaties.PostadresRivendell],
        [TestVertegenwoordigers.Frodo],
        [TestBankrekeningnummers.BankrekeningnummerFrodo],
        [TestErkenningen.LembasBakkenRivendell]
    );

    protected override MultipleWerdGeregistreerdScenario InitializeScenario() =>
        new(GegevensDubbeleVereniging, GegevensAuthentiekeVereniging);

    public MarkeerAlsDubbelVanContext(FullBlownApiSetup apiSetup)
        : base(apiSetup) { }

    protected override async ValueTask ExecuteScenario(MultipleWerdGeregistreerdScenario scenario)
    {
        CommandResult = await new MarkeerAlsDubbelVanRequestFactory(scenario).ExecuteRequest(ApiSetup);

        using var scope = ApiSetup.AdminApiHost.Services.CreateScope();
        await using var session = scope.ServiceProvider.GetRequiredService<IDocumentSession>();

        var stream = await session.Events.FetchStreamAsync(scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode);
        var counter = 0;

        VerenigingAanvaarddeDubbeleVereniging = stream.SingleOrDefault(x =>
            x.EventType == typeof(VerenigingAanvaarddeDubbeleVereniging)
        );

        while (VerenigingAanvaarddeDubbeleVereniging is null && counter < 200)
        {
            counter++;
            await Task.Delay(500);

            stream = await session.Events.FetchStreamAsync(scenario.AndereFeitelijkeVerenigingWerdGeregistreerd.VCode);

            VerenigingAanvaarddeDubbeleVereniging = stream.SingleOrDefault(x =>
                x.EventType == typeof(VerenigingAanvaarddeDubbeleVereniging)
            );
        }

        VerenigingAanvaarddeDubbeleVereniging.Should().NotBeNull();

#pragma warning disable CS0618
        DubbeleVerenigingWerdSamengevoegd = stream.SingleOrDefault(x =>
            x.Data is DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging
        );
#pragma warning restore CS0618

        DubbeleVerenigingWerdSamengevoegd.Should().NotBeNull();

        VerenigingWerdErkend = stream.SingleOrDefault(x => x.Data is VerenigingWerdErkend);

        VerenigingWerdErkend.Should().NotBeNull();
    }
}
