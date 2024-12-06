namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Admin.Schema.PowerBiExport;
using Events;
using Formats;
using KellermanSoftware.CompareNetObjects;
using Marten;
using Publiek.Detail;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdGewijzigd(PowerBiScenarioFixture<LidmaatschapWerdGewijzigdScenario> fixture)
    : PowerBiScenarioClassFixture<LidmaatschapWerdGewijzigdScenario>
{
    [Fact]
    public async Task ARecordIsStored_With_Lidmaatschap()
    {
        Lidmaatschap[] expectedLidmaatschap =
        [
            new(
                fixture.Scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.LidmaatschapId,
                fixture.Scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.AndereVereniging,
                fixture.Scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.DatumVan.FormatAsBelgianDate(),
                fixture.Scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.DatumTot.FormatAsBelgianDate(),
                fixture.Scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.Identificatie,
                fixture.Scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.Beschrijving
            ),
        ];

        fixture.Result.Lidmaatschappen.ShouldCompare(expectedLidmaatschap);
    }

    [Fact]
    public async Task ARecordIsStored_With_Historiek()
    {
        fixture.Result.VCode.Should().Be(fixture.Scenario.LidmaatschapWerdGewijzigd.VCode);
        fixture.Result.Historiek.Should().NotBeEmpty();

        fixture.Result.Historiek.Should()
                             .ContainSingle(x => x.EventType == nameof(LidmaatschapWerdGewijzigd));
    }
}
