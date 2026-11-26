namespace AssociationRegistry.Test.Projections.Beheer.Relatieaanvragen;

using Admin.Schema.Relatieaanvragen;
using Scenario.Relatieaanvragen;

[Collection(nameof(ProjectionContext))]
public class Given_RelatieaanvraagWerdIngediend(
    RelatieaanvraagScenarioFixture<RelatieaanvraagWerdIngediendScenario> fixture
) : RelatieaanvraagScenarioClassFixture<RelatieaanvraagWerdIngediendScenario>
{
    [Fact]
    public void Then_The_Relatieaanvraag_Is_Aangevraagd()
    {
        var ingediend = fixture.Scenario.RelatieaanvraagWerdIngediend;

        fixture
            .Result.Should()
            .BeEquivalentTo(
                new RelatieaanvraagDocument
                {
                    AanvraagId = ingediend.AanvraagId,
                    KoepelVCode = ingediend.KoepelVCode,
                    VerenigingVCode = ingediend.VerenigingVCode,
                    AangevraagdDoor = ingediend.AangevraagdDoor,
                    RelatieType = ingediend.RelatieType,
                    Status = "Aangevraagd",
                    AanvraagDatum = ingediend.AanvraagDatum,
                    VervalDatum = ingediend.VervalDatum,
                    Beschrijving = ingediend.Beschrijving,
                },
                options => options.Excluding(x => x.Metadata)
            );
    }

    [Fact]
    public void Then_There_Is_No_AfhandelDatum() => fixture.Result!.AfhandelDatum.Should().BeNull();

    [Fact]
    public void Then_The_Metadata_Is_Set() => fixture.Result!.Metadata.Version.Should().Be(1);
}
