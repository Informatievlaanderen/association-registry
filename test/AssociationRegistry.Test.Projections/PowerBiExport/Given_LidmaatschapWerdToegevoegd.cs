namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Admin.Schema.PowerBiExport;
using Events;
using Formats;
using KellermanSoftware.CompareNetObjects;
using Marten;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdToegevoegd : IClassFixture<LidmaatschapWerdToegevoegdScenario>
{
    private readonly ProjectionContext _context;
    private readonly LidmaatschapWerdToegevoegdScenario _scenario;

    public Given_LidmaatschapWerdToegevoegd(
        ProjectionContext context,
        LidmaatschapWerdToegevoegdScenario scenario)
    {
        _context = context;
        _scenario = scenario;
    }

    [Fact]
    public async Task ARecordIsStored_With_Lidmaatschap()
    {
        await using var documentSession = _context.Beheer.Session;

        var powerBiExportDocument =
            await documentSession
                 .Query<PowerBiExportDocument>()
                 .Where(w => w.VCode == _scenario.VCodeDochter)
                 .SingleAsync();

        Lidmaatschap[] expectedLidmaatschap =
        [
            new(
                _scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.LidmaatschapId,
                _scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.AndereVereniging,
                _scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.DatumVan.FormatAsBelgianDate(),
                _scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.DatumTot.FormatAsBelgianDate(),
                _scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.Identificatie,
                _scenario.LidmaatschapWerdToegevoegd.Lidmaatschap.Beschrijving
            ),
        ];

        powerBiExportDocument.Lidmaatschappen.ShouldCompare(expectedLidmaatschap);
    }

    [Fact]
    public async Task ARecordIsStored_With_Historiek()
    {
        await using var documentSession = _context.Beheer.Session;

        var powerBiExportDocument =
            await documentSession
                 .Query<PowerBiExportDocument>()
                 .SingleAsync(w => w.VCode == _scenario.LidmaatschapWerdToegevoegd.VCode);

        powerBiExportDocument.VCode.Should().Be(_scenario.LidmaatschapWerdToegevoegd.VCode);
        powerBiExportDocument.Historiek.Should().NotBeEmpty();

        powerBiExportDocument.Historiek.Should()
                             .ContainSingle(x => x.EventType == nameof(LidmaatschapWerdToegevoegd));
    }
}
