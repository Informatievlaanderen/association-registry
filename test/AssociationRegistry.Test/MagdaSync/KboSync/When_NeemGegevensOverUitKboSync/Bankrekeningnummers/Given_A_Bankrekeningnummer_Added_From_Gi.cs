namespace AssociationRegistry.Test.MagdaSync.KboSync.When_NeemGegevensOverUitKboSync.Bankrekeningnummers;

using AssociationRegistry.Magda.Kbo;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using DecentraalBeheer.Vereniging;
using Events;
using FluentAssertions;
using Xunit;

public class Given_A_Bankrekeningnummer_Added_From_Gi
{
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithBankrekeningnummersAddedByGIScenario _scenario;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheid _sut;

    public Given_A_Bankrekeningnummer_Added_From_Gi()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithBankrekeningnummersAddedByGIScenario();

        _sut = new VerenigingMetRechtspersoonlijkheid();
        _sut.Hydrate(_scenario.GetVerenigingState());
    }

    [Fact]
    public void With_Missing_GiBankrekeningnunmmerIban_From_Sync_Then_Dont_Remove_The_Existing_GiBankrekeningnunmmer()
    {
        var verenigingVolgensKbo = _fixture.Create<VerenigingVolgensKbo>() with
        {
            Bankrekeningnummers =
            [
                _fixture.Create<BankrekeningnummerVolgensKbo>() with
                {
                    Iban = _scenario.BankrekeningnummerWerdToegevoegdVanuitKBO1.Iban,
                },
            ],
        };

        _sut.NeemGegevensOverUitKboSync(VerenigingVolgensKboResult.GeldigeVereniging(verenigingVolgensKbo));

        ShouldHaveNotSavedEvents(
            _sut,
            typeof(BankrekeningnummerWerdToegevoegdVanuitKBO),
            typeof(BankrekeningnummerWerdVerwijderdUitKBO)
        );
    }

    [Fact]
    public void With_GiBankrekeningnunmmerIban_From_Sync_Then_BankrekeningnummerWerdOvergenomenVanuitKBO_Event()
    {
        var verenigingVolgensKbo = _fixture.Create<VerenigingVolgensKbo>() with
        {
            Bankrekeningnummers =
            [
                _fixture.Create<BankrekeningnummerVolgensKbo>() with
                {
                    Iban = _scenario.BankrekeningnummerWerdToegevoegdVanuitKBO1.Iban,
                },
                _fixture.Create<BankrekeningnummerVolgensKbo>() with
                {
                    Iban = _scenario.GIBankrekeningnummerWerdToegevoegd.Iban,
                },
            ],
        };

        _sut.NeemGegevensOverUitKboSync(VerenigingVolgensKboResult.GeldigeVereniging(verenigingVolgensKbo));

        ShouldHaveEvents<BankrekeningnummerWerdOvergenomenVanuitKBO>(
            _sut,
            [
                new BankrekeningnummerWerdOvergenomenVanuitKBO(
                    _scenario.GIBankrekeningnummerWerdToegevoegd.BankrekeningnummerId,
                    _scenario.GIBankrekeningnummerWerdToegevoegd.Iban
                ),
            ]
        );
        ShouldHaveNotSavedEvents(
            _sut,
            typeof(BankrekeningnummerWerdToegevoegdVanuitKBO),
            typeof(BankrekeningnummerWerdVerwijderdUitKBO)
        );
    }

    private static void ShouldHaveEvents<T>(VerenigingMetRechtspersoonlijkheid sut, IEnumerable<T> events)
    {
        sut.UncommittedEvents.OfType<T>().Should().AllBeOfType<T>().And.BeEquivalentTo(events);
    }

    private static void ShouldHaveNotSavedEvents(VerenigingMetRechtspersoonlijkheid sut, params Type[] types)
    {
        foreach (var type in types)
        {
            sut.UncommittedEvents.Where(e => e.GetType() == type)
                .Should()
                .BeEmpty($"Expected no uncommitted events of type {type.Name}");
        }
    }
}
