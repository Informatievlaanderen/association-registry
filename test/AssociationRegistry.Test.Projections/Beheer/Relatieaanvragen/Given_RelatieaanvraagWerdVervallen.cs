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

    [Fact]
    public void Then_There_Is_No_Bevestiging_Weigering_Or_Intrekking()
    {
        fixture.Result!.Bevestiging.Should().BeNull();
        fixture.Result!.Weigering.Should().BeNull();
        fixture.Result!.Intrekking.Should().BeNull();
    }

    [Fact]
    public void Then_The_Metadata_Is_Set() => fixture.Result!.Metadata.Version.Should().Be(2);
}
