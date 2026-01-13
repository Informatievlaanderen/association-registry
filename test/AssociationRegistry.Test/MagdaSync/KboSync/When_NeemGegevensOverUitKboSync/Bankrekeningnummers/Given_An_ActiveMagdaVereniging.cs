namespace AssociationRegistry.Test.MagdaSync.KboSync.When_NeemGegevensOverUitKboSync.Bankrekeningnummers;

using AssociationRegistry.Magda.Kbo;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Bankrekeningen;
using Events;
using Events.Factories;
using FluentAssertions;
using Xunit;

public class Given_An_ActiveMagdaVereniging
{
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithBankrekeningnummersScenario _scenario;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheid _sut;

    public Given_An_ActiveMagdaVereniging()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithBankrekeningnummersScenario();

        _sut = new VerenigingMetRechtspersoonlijkheid();
        _sut.Hydrate(_scenario.GetVerenigingState());
    }

    [Fact]
    public void With_No_Changes_Then_Nothing()
    {
        var (verenigingVolgensKbo, _, _) =
            CreateVerenigingVolgensKbo(
                extraBankrekeningnummers: 0,
                nietGewijzigdeBankrekeningnummers:
                [
                    _scenario.BankrekeningnummerWerdToegevoegdVanuitKBO1,
                    _scenario.BankrekeningnummerWerdToegevoegdVanuitKBO2,
                    _scenario.BankrekeningnummerWerdToegevoegdVanuitKBO3,
                ],
                ontbrekendeBankrekeningnummers: []);

        _sut.NeemGegevensOverUitKboSync(VerenigingVolgensKboResult.GeldigeVereniging(verenigingVolgensKbo));

        ShouldHaveNotSavedEvents(_sut,
                                 typeof(BankrekeningnummerWerdToegevoegdVanuitKBO),
                                 typeof(BankrekeningnummerWerdVerwijderdUitKBO));
    }

    [Fact]
    public void With_Extra_Bankrekeningnummers_Then_It_Creates_BankrekeningnummerWerdToegevoegdVanuitKBO_Events()
    {
        var (verenigingVolgensKbo, extraBankrekeningnummers, _) =
            CreateVerenigingVolgensKbo(
                extraBankrekeningnummers: 3,
                nietGewijzigdeBankrekeningnummers:
                [
                    _scenario.BankrekeningnummerWerdToegevoegdVanuitKBO1,
                    _scenario.BankrekeningnummerWerdToegevoegdVanuitKBO2,
                    _scenario.BankrekeningnummerWerdToegevoegdVanuitKBO3,
                ],
                ontbrekendeBankrekeningnummers: []);

        _sut.NeemGegevensOverUitKboSync(VerenigingVolgensKboResult.GeldigeVereniging(verenigingVolgensKbo));

        ShouldHaveEvents(_sut, CreateBankrekeningnummerWerdToegevoegdVanuitKBOEvents(extraBankrekeningnummers));
        ShouldHaveNotSavedEvents(_sut, typeof(BankrekeningnummerWerdVerwijderdUitKBO));
    }

    [Fact]
    public void With_3_Extra_Bankrekeningnummers_With_Same_IBAN_Then_It_Creates_Only_One_BankrekeningnummerWerdToegevoegdUitKBO_Event()
    {
        var dezelfdeIban = _fixture.Create<IbanNummer>();

        var verenigingVolgensKbo = _fixture.Create<VerenigingVolgensKbo>() with
        {
            Bankrekeningnummers = _fixture
                                .CreateMany<BankrekeningnummerVolgensKbo>()
                                .Select(v => v with { Iban = dezelfdeIban.Value })
                                .ToArray(),
        };

        _sut.NeemGegevensOverUitKboSync(VerenigingVolgensKboResult.GeldigeVereniging(verenigingVolgensKbo));

        ShouldHaveEvents(_sut, CreateBankrekeningnummerWerdToegevoegdVanuitKBOEvents([verenigingVolgensKbo.Bankrekeningnummers.First()]));
    }

    [Fact]
    public void With_Missing_Bankrekeningnummers_Then_Foreach_Missing_Bankrekeningnummers_A_BankrekeningnummerWerdVerwijderdUitKBO_Event()
    {
        var (verenigingVolgensKbo, _, teVerwijderenBankrekeningnummers) = CreateVerenigingVolgensKbo(
            extraBankrekeningnummers: 0,
            nietGewijzigdeBankrekeningnummers: [
                _scenario.BankrekeningnummerWerdToegevoegdVanuitKBO3,
            ],
            ontbrekendeBankrekeningnummers: [
                _scenario.BankrekeningnummerWerdToegevoegdVanuitKBO1,
                _scenario.BankrekeningnummerWerdToegevoegdVanuitKBO2,
            ]);

        _sut.NeemGegevensOverUitKboSync(VerenigingVolgensKboResult.GeldigeVereniging(verenigingVolgensKbo));

        var bankrekeningnummersVerwijderdUitKbo = CreateBankrekeningnummerWerdVerwijderdUitKBOEvents(teVerwijderenBankrekeningnummers);

        ShouldHaveEvents(_sut, bankrekeningnummersVerwijderdUitKbo);
        ShouldHaveNotSavedEvents(_sut, typeof(BankrekeningnummerWerdToegevoegdVanuitKBO));
    }

    private (VerenigingVolgensKbo verenigingVolgensKbo, BankrekeningnummerVolgensKbo[] toeTeVoegenBankrekeningnummers, BankrekeningnummerVolgensKbo[] teVerwijderenBankrekeningnummers)
        CreateVerenigingVolgensKbo(
            int extraBankrekeningnummers,
            IEnumerable<BankrekeningnummerWerdToegevoegdVanuitKBO> nietGewijzigdeBankrekeningnummers,
            IEnumerable<BankrekeningnummerWerdToegevoegdVanuitKBO> ontbrekendeBankrekeningnummers)
    {
        var toeTeVoegenBankrekeningnummers = _fixture.CreateMany<BankrekeningnummerVolgensKbo>(extraBankrekeningnummers)
                                                    .ToArray();

        var nietGewijzigdeKboBankrekeningnummers = nietGewijzigdeBankrekeningnummers
            .Select(CreateExistingBankrekeningnummersVolgensKboFromScenario)
            .ToArray();

        var teVerwijderenBankrekeningnummers = ontbrekendeBankrekeningnummers
                                             .Select(CreateExistingBankrekeningnummersVolgensKboFromScenario)
                                             .ToArray();

        var alleBankrekeningnummersVolgensKbo = nietGewijzigdeKboBankrekeningnummers
            .Concat(toeTeVoegenBankrekeningnummers)
            .ToArray();

        var verenigingVolgensKbo = _fixture.Create<VerenigingVolgensKbo>() with
        {
            Bankrekeningnummers = alleBankrekeningnummersVolgensKbo,
        };

        return (verenigingVolgensKbo, toeTeVoegenBankrekeningnummers, teVerwijderenBankrekeningnummers);
    }

    private BankrekeningnummerVolgensKbo CreateExistingBankrekeningnummersVolgensKboFromScenario(BankrekeningnummerWerdToegevoegdVanuitKBO bankrekeningnummerWerdToegevoegdVanuitKbo)
        => new()
        {
            Iban = bankrekeningnummerWerdToegevoegdVanuitKbo.Iban,
        };

    private IEnumerable<BankrekeningnummerWerdVerwijderdUitKBO> CreateBankrekeningnummerWerdVerwijderdUitKBOEvents(BankrekeningnummerVolgensKbo[] bankrekeningnummersVolgensKbo)
    {
        return bankrekeningnummersVolgensKbo.Select((x, i) =>
                                                       EventFactory.BankrekeningnummerWerdVerwijderdUitKBO(
                                                           Bankrekeningnummer.CreateFromKbo(x, _scenario.GetVerenigingState().Bankrekeningnummers
                                                                                               .Single(y => x.Iban == y.Iban.Value).BankrekeningnummerId)));
    }

    private IEnumerable<BankrekeningnummerWerdToegevoegdVanuitKBO> CreateBankrekeningnummerWerdToegevoegdVanuitKBOEvents(BankrekeningnummerVolgensKbo[] bankrekeningnummersVolgensKbo)
    {
        var nextId = _scenario.GetVerenigingState().Bankrekeningnummers.NextId;

        return bankrekeningnummersVolgensKbo.Select(x =>
                                                       EventFactory.BankrekeningnummerWerdToegevoegdVanuitKBO(
                                                           Bankrekeningnummer.CreateFromKbo(x, nextId++)));
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
