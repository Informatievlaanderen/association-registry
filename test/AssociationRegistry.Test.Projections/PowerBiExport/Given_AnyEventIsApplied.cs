namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Admin.Schema.PowerBiExport;
using FluentAssertions;
using Framework;
using Marten;
using ScenarioClassFixtures;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_AnyEventIsApplied : IClassFixture<ApplyAllEventsScenario>
{
    private readonly ProjectionContext _context;
    private readonly ApplyAllEventsScenario _scenario;

    public Given_AnyEventIsApplied(
        ProjectionContext context,
        ApplyAllEventsScenario scenario)
    {
        _context = context;
        _scenario = scenario;
    }

    [Fact]
    public async Task AGebeurtenisIsAddedForEachEvent()
    {
        await using var documentSession = _context
           .Session;

        var powerBiExportDocument =
            await documentSession
                 .Query<PowerBiExportDocument>()
                 .Where(doc => doc.VCode == _scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode)
                 .SingleAsync();

        powerBiExportDocument.Historiek.Should().NotBeEmpty();
        powerBiExportDocument.Historiek[0].EventType.Should().Be(_scenario.FeitelijkeVerenigingWerdGeregistreerd.GetType().Name);
    }
}
