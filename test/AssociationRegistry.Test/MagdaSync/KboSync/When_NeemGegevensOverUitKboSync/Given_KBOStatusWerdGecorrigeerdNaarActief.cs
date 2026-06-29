namespace AssociationRegistry.Test.MagdaSync.KboSync.When_NeemGegevensOverUitKboSync;

using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using DecentraalBeheer.Vereniging;
using Events;
using FluentAssertions;
using Xunit;

public class Given_KBOStatusWerdGecorrigeerdNaarActief
{
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithKBOStatusWerdGecorrigeerdNaarActiefScenario _scenario;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheid _sut;

    public Given_KBOStatusWerdGecorrigeerdNaarActief()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario =
            new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithKBOStatusWerdGecorrigeerdNaarActiefScenario();

        _sut = new VerenigingMetRechtspersoonlijkheid();
        _sut.Hydrate(_scenario.GetVerenigingState());
    }

    [Fact]
    public void With_NaamWerdGewijzigd_Then_State_Is_Actief()
    {
        _scenario.GetVerenigingState().VerenigingStatus.Should().Be(VerenigingStatus.Actief);
    }

    [Fact]
    public void Then_Einddatum_Is_Null()
    {
        _scenario.GetVerenigingState().Einddatum.Should().BeNull();
    }

    [Fact]
    public void With_WijzigNaamUitKobo_Then_Saves_NaamWerdGewijzigdInKbo()
    {
        var verenigingsNaam = _fixture.Create<VerenigingsNaam>();
        _sut.WijzigNaamUitKbo(verenigingsNaam);

        _sut.UncommittedEvents.Should().ContainEquivalentOf(new NaamWerdGewijzigdInKbo(verenigingsNaam));
    }
}
