namespace AssociationRegistry.Test.KboSync.When_NeemGegevensOverUitKboSync.Vertegenwoordigers;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Events.Factories;
using AssociationRegistry.Magda.Kbo;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_An_ActiveMagdaVereniging
{
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithVertegenwoordigersScenario _scenario;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheid _sut;

    public Given_An_ActiveMagdaVereniging()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithVertegenwoordigersScenario();

        _sut = new VerenigingMetRechtspersoonlijkheid();
        _sut.Hydrate(_scenario.GetVerenigingState());
    }

    [Fact]
    public void With_No_Changes_Then_Nothing()
    {
        var (verenigingVolgensKbo, _, _, _) =
            CreateVerenigingVolgensKbo(
                extraVertegenwoordigers: 0,
                gewijzigdeVertegenwoordigers: [],
                nietGewijzigdeVertegenwoordigers:
                [
                    _scenario.VertegenwoordigerWerdToegevoegdVanuitKBO1,
                    _scenario.VertegenwoordigerWerdToegevoegdVanuitKBO2,
                    _scenario.VertegenwoordigerWerdToegevoegdVanuitKBO3,
                ],
                ontbrekendeVertegenwoordigers: []);

        _sut.NeemGegevensOverUitKboSync(VerenigingVolgensKboResult.GeldigeVereniging(verenigingVolgensKbo));

        ShouldHaveNotSavedEvents(_sut,
                                 typeof(VertegenwoordigerWerdToegevoegdVanuitKBO),
                                 typeof(VertegenwoordigerWerdGewijzigdInKBO),
                                 typeof(VertegenwoordigerWerdVerwijderdUitKBO));
    }

    [Fact]
    public void With_Extra_Vertegenwoordigers_Then_It_Creates_VertegenwoordigerWerdToegevoegdVanuitKBO_Events()
    {
        var (verenigingVolgensKbo, extraVertegenwoordigers, _, _) =
            CreateVerenigingVolgensKbo(
                extraVertegenwoordigers: 3,
                gewijzigdeVertegenwoordigers: [],
                nietGewijzigdeVertegenwoordigers:
                [
                    _scenario.VertegenwoordigerWerdToegevoegdVanuitKBO1,
                    _scenario.VertegenwoordigerWerdToegevoegdVanuitKBO2,
                    _scenario.VertegenwoordigerWerdToegevoegdVanuitKBO3,
                ],
                ontbrekendeVertegenwoordigers: []);

        _sut.NeemGegevensOverUitKboSync(VerenigingVolgensKboResult.GeldigeVereniging(verenigingVolgensKbo));

        ShouldHaveEvents(_sut, CreateVertegenwoordigerWerdToegevoegdVanuitKBOEvents(extraVertegenwoordigers));
        ShouldHaveNotSavedEvents(_sut, typeof(VertegenwoordigerWerdGewijzigdInKBO), typeof(VertegenwoordigerWerdVerwijderdUitKBO));
    }

    [Fact]
    public void With_1_Difference_Then_One_VertegenwoordigerWerdGewijzigdInKBO_Event()
    {
        var (verenigingVolgensKbo,_ , gewijzigdeVertegenwoordigers, _) =
            CreateVerenigingVolgensKbo(
                extraVertegenwoordigers: 0,
                gewijzigdeVertegenwoordigers:
                [
                    _scenario.VertegenwoordigerWerdToegevoegdVanuitKBO1
                ],
                nietGewijzigdeVertegenwoordigers:
                [
                    _scenario.VertegenwoordigerWerdToegevoegdVanuitKBO2,
                    _scenario.VertegenwoordigerWerdToegevoegdVanuitKBO3
                ],
                ontbrekendeVertegenwoordigers: []);

        _sut.NeemGegevensOverUitKboSync(VerenigingVolgensKboResult.GeldigeVereniging(verenigingVolgensKbo));

        ShouldHaveEvents(_sut, CreateVertegenwoordigerWerdGewijzigdInKBOEvents(gewijzigdeVertegenwoordigers));
        ShouldHaveNotSavedEvents(_sut, typeof(VertegenwoordigerWerdToegevoegdVanuitKBO), typeof(VertegenwoordigerWerdVerwijderdUitKBO));
    }

    [Fact]
    public void With_All_Differences_Then_Foreach_Vertegenwoordiger_A_VertegenwoordigerWerdGewijzigdInKBO_Event()
    {
        var (verenigingVolgensKbo, _, gewijzigdeVertegenwoordigers, _) = CreateVerenigingVolgensKbo(
            extraVertegenwoordigers: 0,
            gewijzigdeVertegenwoordigers:
            [
                _scenario.VertegenwoordigerWerdToegevoegdVanuitKBO1,
                _scenario.VertegenwoordigerWerdToegevoegdVanuitKBO2,
                _scenario.VertegenwoordigerWerdToegevoegdVanuitKBO3,
            ],
            nietGewijzigdeVertegenwoordigers: [],
            ontbrekendeVertegenwoordigers: []);

        _sut.NeemGegevensOverUitKboSync(VerenigingVolgensKboResult.GeldigeVereniging(verenigingVolgensKbo));

        var vertegenwoordigerWerdGewijzigdInKboEvents = CreateVertegenwoordigerWerdGewijzigdInKBOEvents(gewijzigdeVertegenwoordigers);

        ShouldHaveEvents(_sut, vertegenwoordigerWerdGewijzigdInKboEvents);
        ShouldHaveNotSavedEvents(_sut, typeof(VertegenwoordigerWerdToegevoegdVanuitKBO), typeof(VertegenwoordigerWerdVerwijderdUitKBO));
    }

    [Fact]
    public void With_Missing_Vertegenwoordigers_Then_Foreach_Missing_Vertegenwoordiger_A_VertegenwoordigerWerdVerwijderdUitKBO_Event()
    {
        var (verenigingVolgensKbo, _, _, teVerwijderenVertegenwoordigers) = CreateVerenigingVolgensKbo(
            extraVertegenwoordigers: 0,
            gewijzigdeVertegenwoordigers:
            [],
            nietGewijzigdeVertegenwoordigers: [
                _scenario.VertegenwoordigerWerdToegevoegdVanuitKBO3,
            ],
            ontbrekendeVertegenwoordigers: [
                _scenario.VertegenwoordigerWerdToegevoegdVanuitKBO1,
                _scenario.VertegenwoordigerWerdToegevoegdVanuitKBO2,
            ]);

        _sut.NeemGegevensOverUitKboSync(VerenigingVolgensKboResult.GeldigeVereniging(verenigingVolgensKbo));

        var vertegenwoordigerWerdVerwijderdUitKbo = CreateVertegenwoordigerWerdVerwijderdUitKBO(teVerwijderenVertegenwoordigers);

        ShouldHaveEvents(_sut, vertegenwoordigerWerdVerwijderdUitKbo);
        ShouldHaveNotSavedEvents(_sut, typeof(VertegenwoordigerWerdToegevoegdVanuitKBO), typeof(VertegenwoordigerWerdGewijzigdInKBO));
    }

    private (VerenigingVolgensKbo verenigingVolgensKbo,
        VertegenwoordigerVolgensKbo[] toeTeVoegenVertegenwoordigers,
        VertegenwoordigerVolgensKbo[] gewijzigdeVertegenwoordigers,
        VertegenwoordigerVolgensKbo[] teVerwijderenVertegenwoordigers)
        CreateVerenigingVolgensKbo(
            int extraVertegenwoordigers,
            IEnumerable<VertegenwoordigerWerdToegevoegdVanuitKBO> gewijzigdeVertegenwoordigers,
            IEnumerable<VertegenwoordigerWerdToegevoegdVanuitKBO> nietGewijzigdeVertegenwoordigers,
            IEnumerable<VertegenwoordigerWerdToegevoegdVanuitKBO> ontbrekendeVertegenwoordigers)
    {
        var toeTeVoegenVertegenwoordigers = _fixture.CreateMany<VertegenwoordigerVolgensKbo>(extraVertegenwoordigers)
                                                    .ToArray();

        var nietGewijzigdeKboVertegenwoordigers = nietGewijzigdeVertegenwoordigers
            .Select(CreateExistingWithoutChangesVertegenwoordigerVolgensKboFromScenario)
            .ToArray();

        var gewijzigdeKboVertegenwoordigers = gewijzigdeVertegenwoordigers
            .Select(CreateExistingWithChangesVertegenwoordigerVolgensKboFromScenario)
            .ToArray();

        var teVerwijderenVertegenwoordigers = ontbrekendeVertegenwoordigers
                                             .Select(CreateExistingWithoutChangesVertegenwoordigerVolgensKboFromScenario)
                                             .ToArray();

        var alleVertegenwoordigersVolgensKbo = nietGewijzigdeKboVertegenwoordigers
            .Concat(toeTeVoegenVertegenwoordigers)
            .Concat(gewijzigdeKboVertegenwoordigers)
            .ToArray();

        var verenigingVolgensKbo = _fixture.Create<VerenigingVolgensKbo>() with
        {
            Vertegenwoordigers = alleVertegenwoordigersVolgensKbo,
        };

        return (verenigingVolgensKbo, toeTeVoegenVertegenwoordigers, gewijzigdeKboVertegenwoordigers, teVerwijderenVertegenwoordigers);
    }

    private VertegenwoordigerVolgensKbo CreateExistingWithChangesVertegenwoordigerVolgensKboFromScenario(VertegenwoordigerWerdToegevoegdVanuitKBO vertegenwoordigerWerdToegevoegd)
        => new()
        {
            Insz = vertegenwoordigerWerdToegevoegd.Insz,
            Voornaam = _fixture.Create<Voornaam>(),
            Achternaam = _fixture.Create<Achternaam>(),
        };

    private VertegenwoordigerVolgensKbo CreateExistingWithoutChangesVertegenwoordigerVolgensKboFromScenario(VertegenwoordigerWerdToegevoegdVanuitKBO vertegenwoordigerWerdToegevoegd)
        => new()
        {
            Insz = vertegenwoordigerWerdToegevoegd.Insz,
            Voornaam = vertegenwoordigerWerdToegevoegd.Voornaam,
            Achternaam = vertegenwoordigerWerdToegevoegd.Achternaam,
        };

    private IEnumerable<VertegenwoordigerWerdGewijzigdInKBO> CreateVertegenwoordigerWerdGewijzigdInKBOEvents(VertegenwoordigerVolgensKbo[] gewijzigdeVertegenwoordigers)
    {
        return gewijzigdeVertegenwoordigers.Select(x =>
                                                       EventFactory.VertegenwoordigerWerdGewijzigdInKBO(
                                                           Vertegenwoordiger.CreateFromKbo(x) with
                                                           {
                                                               VertegenwoordigerId = _scenario.GetVerenigingState().Vertegenwoordigers
                                                                  .Single(y => x.Insz == y.Insz).VertegenwoordigerId,
                                                           }));
    }

    private IEnumerable<VertegenwoordigerWerdVerwijderdUitKBO> CreateVertegenwoordigerWerdVerwijderdUitKBO(VertegenwoordigerVolgensKbo[] gewijzigdeVertegenwoordigers)
    {
        return gewijzigdeVertegenwoordigers.Select((x, i) =>
                                                       EventFactory.VertegenwoordigerWerdVerwijderdUitKBO(
                                                           Vertegenwoordiger.CreateFromKbo(x) with
                                                           {
                                                               VertegenwoordigerId = _scenario.GetVerenigingState().Vertegenwoordigers
                                                                  .Single(y => x.Insz == y.Insz).VertegenwoordigerId,
                                                           }));
    }

    private IEnumerable<VertegenwoordigerWerdToegevoegdVanuitKBO> CreateVertegenwoordigerWerdToegevoegdVanuitKBOEvents(VertegenwoordigerVolgensKbo[] gewijzigdeVertegenwoordigers)
    {
        var nextId = _scenario.GetVerenigingState().Vertegenwoordigers.NextId;

        return gewijzigdeVertegenwoordigers.Select(x =>
                                                       EventFactory.VertegenwoordigerWerdToegevoegdVanuitKbo(
                                                           Vertegenwoordiger.CreateFromKbo(x) with
                                                           {
                                                               VertegenwoordigerId = nextId++,
                                                           }));
    }

    private static void ShouldHaveEvents<T>(VerenigingMetRechtspersoonlijkheid sut, IEnumerable<T> events)
    {
        sut.UncommittedEvents.OfType<T>()
           .Should()
           .AllBeOfType<T>()
           .And
           .BeEquivalentTo(events);
    }

    private static void ShouldHaveNotSavedEvents(VerenigingMetRechtspersoonlijkheid sut, params Type[] types)
    {
        foreach (var type in types)
        {
            sut.UncommittedEvents
               .Where(e => e.GetType() == type)
               .Should()
               .BeEmpty($"Expected no uncommitted events of type {type.Name}");
        }
    }
}
