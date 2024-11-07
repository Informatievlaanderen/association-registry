﻿namespace AssociationRegistry.Test.Admin.Api.Projections.V2.PowerBiExport;

using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Admin.Schema.PowerBiExport;
using Beheer.ScenarioClassFixtures;
using Events;
using FluentAssertions;
using KellermanSoftware.CompareNetObjects;
using Marten;
using Projections.PowerBiExport;
using ScenarioClassFixtures;
using Xunit;
using LidmaatschapWerdVerwijderdScenario = ScenarioClassFixtures.LidmaatschapWerdVerwijderdScenario;

[Collection(nameof(PowerBiExportContext))]
public class Given_LidmaatschapWerdVerwijderd : IClassFixture<LidmaatschapWerdVerwijderdScenario>
{
    private readonly PowerBiExportContext _context;
    private readonly LidmaatschapWerdVerwijderdScenario _scenario;

    public Given_LidmaatschapWerdVerwijderd(
        PowerBiExportContext context,
        LidmaatschapWerdVerwijderdScenario scenario)
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

        powerBiExportDocument.Lidmaatschappen.Should().BeEmpty();
    }

    [Fact]
    public async Task ARecordIsStored_With_Historiek()
    {
        await using var documentSession = _context
           .Session;

        var powerBiExportDocument =
            await documentSession
                 .Query<PowerBiExportDocument>()
                 .SingleAsync(w => w.VCode == _scenario.LidmaatschapWerdVerwijderd.VCode);

        powerBiExportDocument.VCode.Should().Be(_scenario.LidmaatschapWerdVerwijderd.VCode);
        powerBiExportDocument.Historiek.Should().NotBeEmpty();

        powerBiExportDocument.Historiek.Should()
                             .ContainSingle(x => x.EventType == nameof(LidmaatschapWerdVerwijderd));
    }
}
