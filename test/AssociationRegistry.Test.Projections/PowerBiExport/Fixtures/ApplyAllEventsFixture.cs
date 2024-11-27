namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Admin.Schema.PowerBiExport;
using Framework.Fixtures;
using Marten;

public class ApplyAllEventsFixture(ProjectionContext context)
    : ScenarioFixture<ApplyAllEventsScenario, PowerBiExportDocument, ProjectionContext>(context)
{
    protected override async Task<PowerBiExportDocument> GetResultAsync(ApplyAllEventsScenario scenario)
        => await Context.Session
                        .Query<PowerBiExportDocument>()
                        .SingleAsync(w => w.VCode == scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode);
}
