namespace AssociationRegistry.Test.Projections.Beheer.Historiek.KboStatusCorrectie;

using Events;
using Scenario.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_KBOStatusWerdGecorrigeerdNaarActief(
    BeheerHistoriekScenarioFixture<KBOStatusWerdGecorrigeerdNaarActiefScenario> fixture
) : BeheerHistoriekScenarioClassFixture<KBOStatusWerdGecorrigeerdNaarActiefScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(3);

    [Fact]
    public void KBOStatusWerdGecorrigeerdNaarActief_Gebeurtenis_Is_Not_In_Gebeurtenissen()
    {
        var gebeurtenis = fixture.Result.Gebeurtenissen.SingleOrDefault(x =>
            x.Gebeurtenis == nameof(KBOStatusWerdGecorrigeerdNaarActief)
        );
        gebeurtenis.Should().BeNull();
    }

    [Fact]
    public void VerenigingWerdGestoptInKBO_Gebeurtenis_Is_Not_In_Gebeurtenissen()
    {
        var gebeurtenis = fixture.Result.Gebeurtenissen.SingleOrDefault(x =>
            x.Gebeurtenis == nameof(VerenigingWerdGestoptInKBO)
        );
        gebeurtenis.Should().BeNull();
    }
}
