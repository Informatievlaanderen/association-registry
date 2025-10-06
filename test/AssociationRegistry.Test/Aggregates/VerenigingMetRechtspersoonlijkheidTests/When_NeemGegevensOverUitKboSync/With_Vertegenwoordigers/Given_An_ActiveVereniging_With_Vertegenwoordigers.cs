namespace AssociationRegistry.Test.Aggregates.VerenigingMetRechtspersoonlijkheidTests.When_NeemGegevensOverUitKboSync.With_Vertegenwoordigers;

using AssociationRegistry.Magda.Kbo;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using DecentraalBeheer.Vereniging;
using Events;
using Events.Factories;
using FluentAssertions;
using Xunit;

public class Given_An_ActiveVereniging_With_Vertegenwoordigers
{
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithVertegenwoordigersScenario _scenario;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheid _sut;

    public Given_An_ActiveVereniging_With_Vertegenwoordigers()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithVertegenwoordigersScenario();

        _sut = new VerenigingMetRechtspersoonlijkheid();
        _sut.Hydrate(_scenario.GetVerenigingState());
    }

    [Fact]
    public void With_No_Changes_Then_Nothing()
    {
        var (verenigingVolgensKbo, _) =
            CreateVerenigingVolgensKbo(
                gewijzigdeVertegenwoordigers: [],
                nietGewijzigdeVertegenwoordigers:
                [
                    _scenario.VertegenwoordigerWerdToegevoegdVanuitKBO1,
                    _scenario.VertegenwoordigerWerdToegevoegdVanuitKBO2,
                ]);

        _sut.NeemGegevensOverUitKboSync(VerenigingVolgensKboResult.GeldigeVereniging(verenigingVolgensKbo));

        _sut.UncommittedEvents.OfType<VertegenwoordigerWerdGewijzigdInKBO>().Should().BeEmpty();
    }

    [Fact]
    public void With_1_Difference_Then_One_VertegenwoordigerWerdGewijzigdInKBO_Event()
    {
        var (verenigingVolgensKbo, gewijzigdeVertegenwoordigers) =
            CreateVerenigingVolgensKbo(
                gewijzigdeVertegenwoordigers:
                [
                    _scenario.VertegenwoordigerWerdToegevoegdVanuitKBO1
                ],
                nietGewijzigdeVertegenwoordigers:
                [
                    _scenario.VertegenwoordigerWerdToegevoegdVanuitKBO2
                ]);

        _sut.NeemGegevensOverUitKboSync(VerenigingVolgensKboResult.GeldigeVereniging(verenigingVolgensKbo));

        ShouldHaveEvents(_sut, CreateVVertegenwoordigerWerdGewijzigdInKBOEvents(gewijzigdeVertegenwoordigers));
    }

    [Fact]
    public void With_All_Differences_Then_Foreach_Vertegenwoordiger_A_VertegenwoordigerWerdGewijzigdInKBO_Event()
    {
        var (verenigingVolgensKbo, gewijzigdeVertegenwoordigers) = CreateVerenigingVolgensKbo(
            gewijzigdeVertegenwoordigers:
            [
                _scenario.VertegenwoordigerWerdToegevoegdVanuitKBO1,
                _scenario.VertegenwoordigerWerdToegevoegdVanuitKBO2,
            ],
            nietGewijzigdeVertegenwoordigers: []);

        _sut.NeemGegevensOverUitKboSync(VerenigingVolgensKboResult.GeldigeVereniging(verenigingVolgensKbo));

        var vVertegenwoordigerWerdGewijzigdInKboEvents = CreateVVertegenwoordigerWerdGewijzigdInKBOEvents(gewijzigdeVertegenwoordigers);

        ShouldHaveEvents(_sut, vVertegenwoordigerWerdGewijzigdInKboEvents);
    }

    private static IEnumerable<VertegenwoordigerWerdGewijzigdInKBO> CreateVVertegenwoordigerWerdGewijzigdInKBOEvents(VertegenwoordigerVolgensKbo[] gewijzigdeVertegenwoordigers)
    {
        return gewijzigdeVertegenwoordigers.Select((x, i) =>
                                                       EventFactory.VertegenwoordigerWerdGewijzigdInKBO(
                                                           Vertegenwoordiger.CreateFromKbo(x) with
                                                           {
                                                               VertegenwoordigerId = ++i,
                                                           }));
    }

    private (VerenigingVolgensKbo verenigingVolgensKbo, VertegenwoordigerVolgensKbo[] gewijzigdeVertegenwoordigers)
        CreateVerenigingVolgensKbo(
            IEnumerable<VertegenwoordigerWerdToegevoegdVanuitKBO> gewijzigdeVertegenwoordigers,
            IEnumerable<VertegenwoordigerWerdToegevoegdVanuitKBO> nietGewijzigdeVertegenwoordigers)
    {
        var nietGewijzigdeKboVertegenwoordigers = nietGewijzigdeVertegenwoordigers
            .Select(CreateNietGewijzigdeKboVertegenwoordiger)
            .ToArray();

        var gewijzigdeKboVertegenwoordigers = gewijzigdeVertegenwoordigers
            .Select(CreateGewijzigdeKboVertegenwoordiger)
            .ToArray();

        var alleVertegenwoordigersVolgensKbo = nietGewijzigdeKboVertegenwoordigers
            .Concat(gewijzigdeKboVertegenwoordigers)
            .ToArray();

        var verenigingVolgensKbo = _fixture.Create<VerenigingVolgensKbo>() with
        {
            Vertegenwoordigers = alleVertegenwoordigersVolgensKbo,
        };

        return (verenigingVolgensKbo, gewijzigdeKboVertegenwoordigers);
    }

    private VertegenwoordigerVolgensKbo CreateGewijzigdeKboVertegenwoordiger(VertegenwoordigerWerdToegevoegdVanuitKBO vertegenwoordigerWerdToegevoegd)
        => new()
        {
            Insz = vertegenwoordigerWerdToegevoegd.Insz,
            Voornaam = _fixture.Create<Voornaam>(),
            Achternaam = _fixture.Create<Achternaam>(),
        };

    private VertegenwoordigerVolgensKbo CreateNietGewijzigdeKboVertegenwoordiger(VertegenwoordigerWerdToegevoegdVanuitKBO vertegenwoordigerWerdToegevoegd)
        => new()
        {
            Insz = vertegenwoordigerWerdToegevoegd.Insz,
            Voornaam = vertegenwoordigerWerdToegevoegd.Voornaam,
            Achternaam = vertegenwoordigerWerdToegevoegd.Achternaam,
        };

    private static void ShouldHaveEvents(VerenigingMetRechtspersoonlijkheid sut, IEnumerable<VertegenwoordigerWerdGewijzigdInKBO> events)
    {
        sut.UncommittedEvents.OfType<VertegenwoordigerWerdGewijzigdInKBO>()
           .Should()
           .AllBeOfType<VertegenwoordigerWerdGewijzigdInKBO>()
           .And
           .BeEquivalentTo(events);
    }
}
