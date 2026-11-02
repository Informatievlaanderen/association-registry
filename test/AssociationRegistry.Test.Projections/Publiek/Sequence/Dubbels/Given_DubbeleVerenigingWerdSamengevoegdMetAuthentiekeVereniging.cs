namespace AssociationRegistry.Test.Projections.Publiek.Sequence.Dubbels;

using Scenario.Dubbels;

[Collection(nameof(ProjectionContext))]
public class Given_DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging(
    PubliekVerenigingSequenceScenarioFixture<DubbeleVerenigingWerdSamengevoegdWithNewItemsScenario> fixture
) : PubliekVerenigingSequenceScenarioClassFixture<DubbeleVerenigingWerdSamengevoegdWithNewItemsScenario>
{
    [Fact]
    public void Sequence_Should_Match_Count_Of_All_Events() => fixture.Result.Sequence.Should().Be(2);
}
