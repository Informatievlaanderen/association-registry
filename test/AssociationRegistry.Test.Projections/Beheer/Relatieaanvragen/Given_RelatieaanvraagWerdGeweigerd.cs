namespace AssociationRegistry.Test.Projections.Beheer.Relatieaanvragen;

using Admin.Schema.Relatieaanvragen;
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
    public void Then_The_Weigering_Has_The_WeigeringsDatum_And_The_Reden() =>
        fixture
            .Result!.Weigering.Should()
            .BeEquivalentTo(
                new RelatieaanvraagWeigering
                {
                    Datum = fixture.Scenario.RelatieaanvraagWerdGeweigerd.WeigeringsDatum,
                    Reden = fixture.Scenario.RelatieaanvraagWerdGeweigerd.Reden,
                }
            );

    [Fact]
    public void Then_There_Is_No_Bevestiging() => fixture.Result!.Bevestiging.Should().BeNull();

    [Fact]
    public void Then_The_Metadata_Is_Set() => fixture.Result!.Metadata.Version.Should().Be(2);
}
