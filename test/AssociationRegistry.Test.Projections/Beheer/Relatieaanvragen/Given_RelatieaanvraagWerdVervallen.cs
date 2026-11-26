namespace AssociationRegistry.Test.Projections.Beheer.Relatieaanvragen;

using DecentraalBeheer.Relatieaanvragen;
using Formats;
using Scenario.Relatieaanvragen;

[Collection(nameof(ProjectionContext))]
public class Given_RelatieaanvraagWerdVervallen(
    RelatieaanvraagScenarioFixture<RelatieaanvraagWerdVervallenScenario> fixture
) : RelatieaanvraagScenarioClassFixture<RelatieaanvraagWerdVervallenScenario>
{
    [Fact]
    public void Then_The_Relatieaanvraag_Is_Vervallen() =>
        fixture.Result!.Status.Should().Be(RelatieaanvraagStatus.Labels.Vervallen);

    [Fact]
    public void Then_The_VervalDatum_Is_Unchanged() =>
        fixture.Result!.VervalDatum.Should().Be(fixture.Scenario.Ingediend.RelatieaanvraagWerdIngediend.VervalDatum);

    /// <summary>The day it vervallen werd, not the day after its vervaldatum: the job ran later.</summary>
    [Fact]
    public void Then_The_AfhandelDatum_Is_The_Day_It_Vervallen_Werd() =>
        fixture
            .Result!.AfhandelDatum.Should()
            .Be(fixture.Scenario.RelatieaanvraagWerdVervallen.VervalDatum.ToString(WellknownFormats.DateOnly));

    [Fact]
    public void Then_The_Metadata_Is_Set() => fixture.Result!.Metadata.Version.Should().Be(2);
}
