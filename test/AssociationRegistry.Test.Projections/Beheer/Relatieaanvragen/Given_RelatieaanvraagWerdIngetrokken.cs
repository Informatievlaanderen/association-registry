namespace AssociationRegistry.Test.Projections.Beheer.Relatieaanvragen;

using Admin.Schema.Relatieaanvragen;
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
    public void Then_The_Intrekking_Has_The_IntrekkingsDatum_And_The_Reden() =>
        fixture
            .Result!.Intrekking.Should()
            .BeEquivalentTo(
                new RelatieaanvraagIntrekking
                {
                    Datum = fixture.Scenario.RelatieaanvraagWerdIngetrokken.IntrekkingsDatum,
                    Reden = fixture.Scenario.RelatieaanvraagWerdIngetrokken.Reden,
                }
            );

    [Fact]
    public void Then_There_Is_No_Bevestiging_Or_Weigering()
    {
        fixture.Result!.Bevestiging.Should().BeNull();
        fixture.Result!.Weigering.Should().BeNull();
    }

    [Fact]
    public void Then_The_Metadata_Is_Set() => fixture.Result!.Metadata.Version.Should().Be(2);
}
