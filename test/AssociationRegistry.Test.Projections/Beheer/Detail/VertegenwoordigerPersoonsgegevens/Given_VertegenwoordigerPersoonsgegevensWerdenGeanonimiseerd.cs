namespace AssociationRegistry.Test.Projections.Beheer.Detail.VertegenwoordigerPersoonsgegevens;

using AssociationRegistry.Test.Projections.Scenario.Vertegenwoordigers.Vzer;
using Scenario.VertegenwoordigerPersoonsgegevens;

[Collection(nameof(ProjectionContext))]
public class Given_VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd(
    BeheerDetailScenarioFixture<VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerdScenario> fixture
) : BeheerDetailScenarioClassFixture<VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerdScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(3);
}
