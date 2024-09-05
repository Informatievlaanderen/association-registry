namespace AssociationRegistry.Test.Admin.Api.Projections.PowerBiExport;

using AssociationRegistry.Admin.Schema.PowerBiExport;
using FluentAssertions;
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
        var powerBiExportDocument =
            await _context
                 .Session
                 .Query<PowerBiExportDocument>()
                 .Where(doc => doc.VCode == _scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode)
                 .SingleAsync();

        powerBiExportDocument.Historiek.Should().NotBeEmpty();
        powerBiExportDocument.Historiek[0].EventType.Should().Be(_scenario.FeitelijkeVerenigingWerdGeregistreerd.GetType().Name);
    }
}
