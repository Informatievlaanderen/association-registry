﻿namespace AssociationRegistry.Test.Admin.Api.Projections.V2.PowerBiExport;

using AssociationRegistry.Admin.ProjectionHost.Projections.PowerBiExport;
using AssociationRegistry.Admin.Schema.PowerBiExport;
using FluentAssertions;
using Marten;
using ScenarioClassFixtures;
using Xunit;

[Collection(nameof(PowerBiExportContext))]
public class Given_FeitelijkeVerenigingWerdVerwijderd : IClassFixture<FeitelijkeVerenigingWerdVerwijderdScenario>
{
    private readonly PowerBiExportContext _context;
    private readonly FeitelijkeVerenigingWerdVerwijderdScenario _scenario;

    public Given_FeitelijkeVerenigingWerdVerwijderd(
        PowerBiExportContext context,
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
                 .SingleOrDefaultAsync(w => w.VCode == _scenario.FeitelijkeVerenigingWerdGeregistreerd2.VCode);

        vereniging2.Should().NotBeNull();
    }
}
