namespace AssociationRegistry.Test.Projections.PowerBiExport;

using Admin.ProjectionHost.Projections;
using Admin.Schema.PowerBiExport;
using Framework.Fixtures;
using Marten;
using Marten.Events.Daemon;
using System.Threading;
using System.Threading.Tasks;

public class PowerBiScenarioFixture<TScenario>(ProjectionContext context)
    : ScenarioFixture<TScenario, PowerBiExportDocument, ProjectionContext>(context)
    where TScenario : IScenario, new()
{
    protected override IDocumentStore DocumentStore => Context.AdminStore;

    protected override async Task RefreshProjectionsAsync(IProjectionDaemon daemon)
    {
        await daemon.RebuildProjectionAsync(ProjectionNames.PowerBi, CancellationToken.None);
    }

    protected override async Task<PowerBiExportDocument> GetResultAsync(
        IDocumentSession session,
        TScenario scenario)
        => await session
                .Query<PowerBiExportDocument>()
                .SingleAsync(x => x.VCode == scenario.VCode);
}

public class PowerBiScenarioClassFixture<TScenario>
    : IClassFixture<PowerBiScenarioFixture<TScenario>>
    where TScenario : IScenario, new()
{
}
