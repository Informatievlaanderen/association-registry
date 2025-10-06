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

public class Given_An_ActiveMagdaVereniging_With_Less_Vertegenwoordigers
{
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithVertegenwoordigersScenario _scenario;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheid _sut;

    public Given_An_ActiveMagdaVereniging_With_Less_Vertegenwoordigers()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithVertegenwoordigersScenario();

        _sut = new VerenigingMetRechtspersoonlijkheid();
        _sut.Hydrate(_scenario.GetVerenigingState());
    }

    [Fact]
    public void With_No_Changes_Then_Nothing()
    {
        var (verenigingVolgensKbo, _, _) =
            CreateVerenigingVolgensKbo(
                gewijzigdeVertegenwoordigers: [],
                nietGewijzigdeVertegenwoordigers:
                [
                    _scenario.VertegenwoordigerWerdToegevoegdVanuitKBO1,
                    _scenario.VertegenwoordigerWerdToegevoegdVanuitKBO2,
                ],
                ontbrekendeVertegenwoordigers: []);

        _sut.NeemGegevensOverUitKboSync(VerenigingVolgensKboResult.GeldigeVereniging(verenigingVolgensKbo));

        _sut.UncommittedEvents.OfType<VertegenwoordigerWerdGewijzigdInKBO>().Should().BeEmpty();
        _sut.UncommittedEvents.OfType<VertegenwoordigerWerdToegevoegdVanuitKBO>().Should().BeEmpty();
        _sut.UncommittedEvents.OfType<VertegenwoordigerWerdVerwijderdUitKBO>().Should().BeEmpty();
    }

    [Fact]
    public void With_One_Less_Vertegenwoordiger_Then_One_VertegenwoordigerWerdVerwijderdUitKBO_Event()
    {
        var (verenigingVolgensKbo, _, teVerwijderenVertegenwoordigers) =
            CreateVerenigingVolgensKbo(
                gewijzigdeVertegenwoordigers: [],
                nietGewijzigdeVertegenwoordigers: [
                    _scenario.VertegenwoordigerWerdToegevoegdVanuitKBO2,
                ],
                ontbrekendeVertegenwoordigers: [
                    _scenario.VertegenwoordigerWerdToegevoegdVanuitKBO1]
                );

        _sut.NeemGegevensOverUitKboSync(VerenigingVolgensKboResult.GeldigeVereniging(verenigingVolgensKbo));

        var teVerwijderenVertegenwoordigersUitKbo = CreateVertegenwoordigerWerdVerwijderdUitKBO(teVerwijderenVertegenwoordigers);

        ShouldHaveEvents(_sut, teVerwijderenVertegenwoordigersUitKbo);
    }

    private static IEnumerable<VertegenwoordigerWerdVerwijderdUitKBO> CreateVertegenwoordigerWerdVerwijderdUitKBO(VertegenwoordigerVolgensKbo[] gewijzigdeVertegenwoordigers)
    {
        return gewijzigdeVertegenwoordigers.Select((x, i) =>
                                                       EventFactory.VertegenwoordigerWerdVerwijderdUitKBO(
                                                           Vertegenwoordiger.CreateFromKbo(x) with
                                                           {
                                                               VertegenwoordigerId = ++i,
                                                           }));
    }

    private (VerenigingVolgensKbo verenigingVolgensKbo, VertegenwoordigerVolgensKbo[] gewijzigdeVertegenwoordigers, VertegenwoordigerVolgensKbo[] teVerwijderenVertegenwoordigers)
        CreateVerenigingVolgensKbo(
            IEnumerable<VertegenwoordigerWerdToegevoegdVanuitKBO> gewijzigdeVertegenwoordigers,
            IEnumerable<VertegenwoordigerWerdToegevoegdVanuitKBO> nietGewijzigdeVertegenwoordigers,
            IEnumerable<VertegenwoordigerWerdToegevoegdVanuitKBO> ontbrekendeVertegenwoordigers)
    {
        var nietGewijzigdeKboVertegenwoordigers = nietGewijzigdeVertegenwoordigers
            .Select(CreateNietGewijzigdeKboVertegenwoordiger)
            .ToArray();

        var gewijzigdeKboVertegenwoordigers = gewijzigdeVertegenwoordigers
            .Select(CreateVertegenwoordigerVolgensKboFromScenario)
            .ToArray();

        var teVerwijderenVertegenwoordigers = ontbrekendeVertegenwoordigers
                                             .Select(CreateVertegenwoordigerVolgensKboFromScenario)
                                             .ToArray();

        var alleVertegenwoordigersVolgensKbo = nietGewijzigdeKboVertegenwoordigers
            .Concat(gewijzigdeKboVertegenwoordigers)
            .ToArray();

        var verenigingVolgensKbo = _fixture.Create<VerenigingVolgensKbo>() with
        {
            Vertegenwoordigers = alleVertegenwoordigersVolgensKbo,
        };

        return (verenigingVolgensKbo, gewijzigdeKboVertegenwoordigers, teVerwijderenVertegenwoordigers);
    }

    private VertegenwoordigerVolgensKbo CreateVertegenwoordigerVolgensKboFromScenario(VertegenwoordigerWerdToegevoegdVanuitKBO vertegenwoordigerWerdToegevoegd)
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

    private static void ShouldHaveEvents(VerenigingMetRechtspersoonlijkheid sut, IEnumerable<VertegenwoordigerWerdVerwijderdUitKBO> events)
    {
        sut.UncommittedEvents.OfType<VertegenwoordigerWerdGewijzigdInKBO>().Should().BeEmpty();
        sut.UncommittedEvents.OfType<VertegenwoordigerWerdToegevoegdVanuitKBO>().Should().BeEmpty();

        sut.UncommittedEvents.OfType<VertegenwoordigerWerdVerwijderdUitKBO>()
           .Should()
           .AllBeOfType<VertegenwoordigerWerdVerwijderdUitKBO>()
           .And
           .BeEquivalentTo(events);
    }
}
