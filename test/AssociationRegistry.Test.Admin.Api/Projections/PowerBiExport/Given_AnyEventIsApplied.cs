namespace AssociationRegistry.Test.Admin.Api.Projections.PowerBiExport;

using AssociationRegistry.Admin.Schema.PowerBiExport;
using Events;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Marten;
using ScenarioClassFixtures;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_AnyEventIsApplied : IClassFixture<HoofdactiviteitenWerdenGewijzigdScenario>
{
    private readonly ProjectionContext _context;
    private readonly HoofdactiviteitenWerdenGewijzigdScenario _scenario;

    public Given_AnyEventIsApplied(
        ProjectionContext context,
        HoofdactiviteitenWerdenGewijzigdScenario scenario)
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
                 .SingleAsync();

        powerBiExportDocument.VCode.Should().Be(_scenario.VerenigingWerdGeregistreerd.VCode);
        powerBiExportDocument.Historiek.Should().NotBeEmpty();
        powerBiExportDocument.Historiek[0].EventType.Should().Be(_scenario.VerenigingWerdGeregistreerd.GetType().Name);
        powerBiExportDocument.Historiek[0].EventType.Should().Be(_scenario.VerenigingWerdGeregistreerd.GetType().Name);
    }
}
