namespace AssociationRegistry.Test.Projections.Bewaartermijn;

using Admin.Schema.Bewaartermijn;
using Scenario.Bewaartermijnen;

[Collection(nameof(ProjectionContext))]
public class Given_BewaartermijnWerdGestart(
    BewaartermijnScenarioFixture<BewaartermijnWerdGestartScenario> fixture)
    : BewaartermijnScenarioClassFixture<BewaartermijnWerdGestartScenario>
{
    [Fact]
    public void Bewaartermijn_Document_Is_Saved()
        => fixture.Result.Should().BeEquivalentTo(new BewaartermijnDocument(fixture.Scenario.BewaartermijnWerdGestart.BewaartermijnId,
                                                                            fixture.Scenario.BewaartermijnWerdGestart.VCode,
                                                                            fixture.Scenario.BewaartermijnWerdGestart.VertegenwoordigerId,
                                                                            fixture.Scenario.BewaartermijnWerdGestart.Vervaldag));
}
