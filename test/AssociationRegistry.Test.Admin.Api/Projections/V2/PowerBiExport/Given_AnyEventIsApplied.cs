namespace AssociationRegistry.Test.Admin.Api.Projections.V2.PowerBiExport;

using AssociationRegistry.Admin.Schema.PowerBiExport;
using FluentAssertions;
using Marten;
using ScenarioClassFixtures;
using Xunit;

[Collection(nameof(PowerBiExportContext))]
public class Given_AnyEventIsApplied : IClassFixture<ApplyAllEventsScenario>
{
    private readonly PowerBiExportContext _context;
    private readonly ApplyAllEventsScenario _scenario;

    public Given_AnyEventIsApplied(
        PowerBiExportContext context,
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
