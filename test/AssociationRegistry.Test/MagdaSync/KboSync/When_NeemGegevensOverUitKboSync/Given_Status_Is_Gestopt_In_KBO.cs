namespace AssociationRegistry.Test.MagdaSync.KboSync.When_NeemGegevensOverUitKboSync;

using AssociationRegistry.Magda.Kbo;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using DecentraalBeheer.Vereniging;
using Events;
using FluentAssertions;
using Xunit;

public class Given_Status_Is_Gestopt_In_KBO
{
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithStatusGestoptScenario _scenario;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheid _sut;

    public Given_Status_Is_Gestopt_In_KBO()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithStatusGestoptScenario();

        _sut = new VerenigingMetRechtspersoonlijkheid();
        _sut.Hydrate(_scenario.GetVerenigingState());
    }

    [Fact]
    public void With_Actieve_Vereniging_Volgens_Magda_Then_Saves_KBOStatusWerdGecorrigeerdNaarActief()
    {
        var verenigingVolgensMagda = _fixture.Create<VerenigingVolgensKbo>() with { IsActief = true };

        _sut.NeemGegevensOverUitKboSync(VerenigingVolgensKboResult.GeldigeVereniging(verenigingVolgensMagda));

        _sut.UncommittedEvents.OfType<KBOStatusWerdGecorrigeerdNaarActief>()
            .Should()
            .AllBeOfType<KBOStatusWerdGecorrigeerdNaarActief>()
            .And.BeEquivalentTo([new KBOStatusWerdGecorrigeerdNaarActief(_sut.VCode)]);
    }
}
