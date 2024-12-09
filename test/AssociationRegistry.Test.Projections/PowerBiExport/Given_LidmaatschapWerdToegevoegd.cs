namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Admin.Schema.PowerBiExport;
using Events;
using Formats;
using KellermanSoftware.CompareNetObjects;
using Marten;
using Publiek.Detail;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdToegevoegd(PowerBiScenarioFixture<LidmaatschapWerdToegevoegdScenario> fixture)
    : PowerBiScenarioClassFixture<LidmaatschapWerdToegevoegdScenario>
{
    [Fact]
    public void ARecordIsStored_With_Lidmaatschap()
    {
        Lidmaatschap[] expectedLidmaatschap =
        [
            new(
                fixture.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId,
                fixture.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.AndereVereniging,
                fixture.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.DatumVan.FormatAsBelgianDate(),
                fixture.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.DatumTot.FormatAsBelgianDate(),
                fixture.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.Identificatie,
                fixture.Scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.Beschrijving
            ),
        ];

        fixture.Result.Lidmaatschappen.ShouldCompare(expectedLidmaatschap);
    }

    [Fact]
    public void ARecordIsStored_With_Historiek()
    {
        fixture.Result.VCode.Should().Be(fixture.Scenario.LidmaatschapWerdToegevoegd.VCode);
        fixture.Result.Historiek.Should().NotBeEmpty();

        fixture.Result.Historiek.Should()
                             .ContainSingle(x => x.EventType == nameof(LidmaatschapWerdToegevoegd));
    }
}
