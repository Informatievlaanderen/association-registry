namespace AssociationRegistry.Test.Admin.Api.Projections.PowerBiExport;

using AssociationRegistry.Admin.ProjectionHost.Projections.PowerBiExport;
using AssociationRegistry.Admin.Schema.PowerBiExport;
using FluentAssertions;
using Marten;
using ScenarioClassFixtures;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_FeitelijkeVerenigingWerdVerwijderd : IClassFixture<FeitelijkeVerenigingWerdVerwijderdScenario>
{
    private readonly ProjectionContext _context;
    private readonly FeitelijkeVerenigingWerdVerwijderdScenario _scenario;

    public Given_FeitelijkeVerenigingWerdVerwijderd(
        ProjectionContext context,
        FeitelijkeVerenigingWerdVerwijderdScenario scenario)
    {
        _context = context;
        _scenario = scenario;
    }

    [Fact]
    public async Task ARecordIsStored_For_Vereniging1_With_StatusVerwijderd()
    {
        var vereniging1 =
            await _context
                 .Session
                 .Query<PowerBiExportDocument>()
                 .Where(w => w.VCode == _scenario.FeitelijkeVerenigingWerdGeregistreerd1.VCode)
                 .SingleOrDefaultAsync();

        vereniging1.Should().NotBeNull();
        vereniging1.Status.Should().Be(PowerBiExportProjection.StatusVerwijderd);
    }

    [Fact]
    public async Task ARecordIsStored_For_Vereniging2()
    {
        var vereniging2 =
            await _context
                 .Session
                 .Query<PowerBiExportDocument>()
                 .Where(w => w.VCode == _scenario.FeitelijkeVerenigingWerdGeregistreerd2.VCode)
                 .SingleOrDefaultAsync();

        vereniging2.Should().NotBeNull();
    }
}
