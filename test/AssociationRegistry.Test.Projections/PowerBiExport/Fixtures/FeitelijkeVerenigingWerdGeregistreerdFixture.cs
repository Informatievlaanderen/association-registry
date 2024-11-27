namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Admin.Schema.PowerBiExport;
using Framework.Fixtures;
using Marten;

public class FeitelijkeVerenigingWerdGeregistreerdFixture(ProjectionContext context)
    : ScenarioFixture<FeitelijkeVerenigingWerdGeregistreerdScenario, PowerBiExportDocument, ProjectionContext>(context)
{
    protected override async Task<PowerBiExportDocument> GetResultAsync(FeitelijkeVerenigingWerdGeregistreerdScenario scenario)
        => await Context.Beheer.Session
                        .Query<PowerBiExportDocument>()
                        .SingleAsync(w => w.VCode == scenario.VerenigingWerdGeregistreerd.VCode);
}
