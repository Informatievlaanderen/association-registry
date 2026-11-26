namespace AssociationRegistry.Test.Projections.Beheer.Relatieaanvragen;

using DecentraalBeheer.Relatieaanvragen;
using Scenario.Relatieaanvragen;

[Collection(nameof(ProjectionContext))]
public class Given_RelatieaanvraagWerdIngetrokken(
    RelatieaanvraagScenarioFixture<RelatieaanvraagWerdIngetrokkenScenario> fixture
) : RelatieaanvraagScenarioClassFixture<RelatieaanvraagWerdIngetrokkenScenario>
{
    [Fact]
    public void Then_The_Relatieaanvraag_Is_Ingetrokken() =>
        fixture.Result!.Status.Should().Be(RelatieaanvraagStatus.Labels.Ingetrokken);

    [Fact]
    public void Then_The_AfhandelDatum_Is_The_IntrekkingsDatum() =>
        fixture.Result!.AfhandelDatum.Should().Be(fixture.Scenario.RelatieaanvraagWerdIngetrokken.IntrekkingsDatum);

    [Fact]
    public void Then_The_Metadata_Is_Set() => fixture.Result!.Metadata.Version.Should().Be(2);
}
