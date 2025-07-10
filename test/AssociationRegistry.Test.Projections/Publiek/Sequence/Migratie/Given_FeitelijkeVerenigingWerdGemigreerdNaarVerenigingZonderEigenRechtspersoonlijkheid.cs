namespace AssociationRegistry.Test.Projections.Publiek.Sequence.Migratie;

using AssociationRegistry.Public.Schema.Detail;
using AssociationRegistry.Test.Projections.Scenario.Migratie;
using AssociationRegistry.Vereniging;
using Detail;

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
