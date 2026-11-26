namespace AssociationRegistry.Test.Projections.Beheer.Relatieaanvragen;

using DecentraalBeheer.Relatieaanvragen;
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

    /// <summary>A relatieaanvraag is vervallen from the day after its vervaldatum.</summary>
    [Fact]
    public void Then_The_AfhandelDatum_Is_The_Day_After_The_VervalDatum() =>
        fixture
            .Result!.AfhandelDatum.Should()
            .Be(fixture.Scenario.Ingediend.RelatieaanvraagWerdIngediend.VervalDatum.AddDays(1));

    [Fact]
    public void Then_The_Metadata_Is_Set() => fixture.Result!.Metadata.Version.Should().Be(2);
}
