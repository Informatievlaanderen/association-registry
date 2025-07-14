namespace AssociationRegistry.Test.Projections.Publiek.Sequence.Migratie;

using Scenario.Migratie;

[Collection(nameof(ProjectionContext))]
public class Given_FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(
    PubliekVerenigingSequenceScenarioFixture<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario>
        fixture)
    : PubliekVerenigingSequenceScenarioClassFixture<
        FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario>
{
    [Fact]
    public void Sequence_Should_Match_Count_Of_All_Events()
        => fixture.Result.Sequence.Should().Be(2);
}
