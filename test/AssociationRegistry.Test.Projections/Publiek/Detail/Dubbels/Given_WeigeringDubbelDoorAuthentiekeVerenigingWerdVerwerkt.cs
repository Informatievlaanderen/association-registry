namespace AssociationRegistry.Test.Projections.Publiek.Detail.Dubbels;

using Public.Schema.Constants;
using Scenario.Dubbels;

[Collection(nameof(ProjectionContext))]
public class Given_WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt(PubliekDetailScenarioFixture<WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerktScenario> fixture)
    : PubliekDetailScenarioClassFixture<WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerktScenario>
{
    [Fact]
    public void Document_Weigering_Dubbel_Door_Authentieke_Vereniging_Werd_Verwerkt()
        => fixture.Result.Status.Should().Be(VerenigingStatus.Actief);
}
