namespace AssociationRegistry.Test.Projections.Beheer.Relatieaanvragen;

using Admin.Schema.Relatieaanvragen;
using Scenario.Relatieaanvragen;

[Collection(nameof(ProjectionContext))]
public class Given_RelatieaanvraagWerdBevestigd(
    RelatieaanvraagScenarioFixture<RelatieaanvraagWerdBevestigdScenario> fixture
) : RelatieaanvraagScenarioClassFixture<RelatieaanvraagWerdBevestigdScenario>
{
    [Fact]
    public void Then_The_Relatieaanvraag_Is_Bevestigd() => fixture.Result!.Status.Should().Be("Bevestigd");

    [Fact]
    public void Then_The_Bevestiging_Has_The_BevestigingsDatum() =>
        fixture
            .Result!.Bevestiging.Should()
            .BeEquivalentTo(
                new RelatieaanvraagBevestiging
                {
                    Datum = fixture.Scenario.RelatieaanvraagWerdBevestigd.BevestigingsDatum,
                }
            );

    [Fact]
    public void Then_The_Metadata_Is_Set() => fixture.Result!.Metadata.Version.Should().Be(2);
}
