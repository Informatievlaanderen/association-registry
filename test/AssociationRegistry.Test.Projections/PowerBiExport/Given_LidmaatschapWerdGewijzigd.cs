namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Admin.Schema.PowerBiExport;
using Events;
using Formats;
using KellermanSoftware.CompareNetObjects;
using Marten;
using ScenarioClassFixtures;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdGewijzigd : IClassFixture<LidmaatschapWerdGewijzigdScenario>
{
    private readonly ProjectionContext _context;
    private readonly LidmaatschapWerdGewijzigdScenario _scenario;

    public Given_LidmaatschapWerdGewijzigd(
        ProjectionContext context,
        LidmaatschapWerdGewijzigdScenario scenario)
    {
        _context = context;
        _scenario = scenario;
    }

    [Fact]
    public async Task ARecordIsStored_With_Lidmaatschap()
    {
        await using var documentSession = _context
           .Session;

        var powerBiExportDocument =
            await documentSession
                 .Query<PowerBiExportDocument>()
                 .Where(w => w.VCode == _scenario.VCodeDochter)
                 .SingleAsync();

        Lidmaatschap[] expectedLidmaatschap =
        [
            new(
                _scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.LidmaatschapId,
                _scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.AndereVereniging,
                _scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.DatumVan.FormatAsBelgianDate(),
                _scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.DatumTot.FormatAsBelgianDate(),
                _scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.Identificatie,
                _scenario.LidmaatschapWerdGewijzigd.Lidmaatschap.Beschrijving
            ),
        ];

        powerBiExportDocument.Lidmaatschappen.ShouldCompare(expectedLidmaatschap);
    }

    [Fact]
    public async Task ARecordIsStored_With_Historiek()
    {
        await using var documentSession = _context
           .Session;

        var powerBiExportDocument =
            await documentSession
                 .Query<PowerBiExportDocument>()
                 .SingleAsync(w => w.VCode == _scenario.LidmaatschapWerdGewijzigd.VCode);

        powerBiExportDocument.VCode.Should().Be(_scenario.LidmaatschapWerdGewijzigd.VCode);
        powerBiExportDocument.Historiek.Should().NotBeEmpty();

        powerBiExportDocument.Historiek.Should()
                             .ContainSingle(x => x.EventType == nameof(LidmaatschapWerdGewijzigd));
    }
}
