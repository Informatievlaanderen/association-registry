namespace AssociationRegistry.Test.Projections.Bewaartermijn;

using Admin.Schema.Bewaartermijn;
using Formats;
using Framework.Fixtures;
using Scenario.Bewaartermijnen;

[Collection(nameof(ProjectionContext))]
public class Given_BewaartermijnWerdGestart(BewaartermijnScenarioFixture<BewaartermijnWerdGestartScenario> fixture)
    : BewaartermijnScenarioClassFixture<BewaartermijnWerdGestartScenario>
{
    [Fact]
    public void Bewaartermijn_Document_Is_Saved() =>
        fixture
            .Result.Should()
            .BeEquivalentTo(
                new BewaartermijnDocument(
                    fixture.Scenario.BewaartermijnWerdGestartV2.BewaartermijnId,
                    fixture.Scenario.BewaartermijnWerdGestartV2.VCode,
                    fixture.Scenario.BewaartermijnWerdGestartV2.BewaartermijnType,
                    fixture.Scenario.BewaartermijnWerdGestartV2.RecordId,
                    fixture.Scenario.BewaartermijnWerdGestartV2.Reden,
                    BewaartermijnStatus.StatusGepland.Naam,
                    fixture.Scenario.BewaartermijnWerdGestartV2.Vervaldag,
                    [
                        new BewaartermijnGebeurtenis(
                            BewaartermijnStatus.StatusGepland.Naam,
                            fixture.MetadataTijdstip.ToInstant()
                        ),
                    ]
                )
            );
}
