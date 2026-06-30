namespace AssociationRegistry.Test.Projections.Beheer.Detail.KboStatusCorrectie;

using Admin.Schema.Constants;
using Scenario.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_KBOStatusWerdGecorrigeerdNaarActief(
    BeheerDetailScenarioFixture<KBOStatusWerdGecorrigeerdNaarActiefScenario> fixture
) : BeheerDetailScenarioClassFixture<KBOStatusWerdGecorrigeerdNaarActiefScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(3);

    [Fact]
    public void Status_Is_Actief()
    {
        fixture.Result.Status.Should().Be(VerenigingStatus.Actief);
    }

    [Fact]
    public void Einddatum_Is_Null()
    {
        fixture.Result.Einddatum.Should().BeNull();
    }
}
