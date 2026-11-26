namespace AssociationRegistry.Test.Projections.Beheer.Relatieaanvragen;

using DecentraalBeheer.Relatieaanvragen;
using Scenario.Relatieaanvragen;

[Collection(nameof(ProjectionContext))]
public class Given_RelatieaanvraagWerdGeweigerd(
    RelatieaanvraagScenarioFixture<RelatieaanvraagWerdGeweigerdScenario> fixture
) : RelatieaanvraagScenarioClassFixture<RelatieaanvraagWerdGeweigerdScenario>
{
    [Fact]
    public void Then_The_Relatieaanvraag_Is_Geweigerd() =>
        fixture.Result!.Status.Should().Be(RelatieaanvraagStatus.Labels.Geweigerd);

    [Fact]
    public void Then_The_AfhandelDatum_Is_The_WeigeringsDatum() =>
        fixture.Result!.AfhandelDatum.Should().Be(fixture.Scenario.RelatieaanvraagWerdGeweigerd.WeigeringsDatum);

    [Fact]
    public void Then_The_Metadata_Is_Set() => fixture.Result!.Metadata.Version.Should().Be(2);
}
