namespace AssociationRegistry.Test.Projections.Beheer.Detail;

using Admin.ProjectionHost.Projections.Detail;
using Admin.Schema.Detail;
using Framework.Fixtures;
using Marten;
using Marten.Events.Daemon;

public class BeheerDetailScenarioFixture<TScenario>(ProjectionContext context)
    : ScenarioFixture<TScenario, BeheerVerenigingDetailDocument, ProjectionContext>(context)
    where TScenario : IScenario, new()
{
    protected override async Task<BeheerVerenigingDetailDocument> GetResultAsync(
        TScenario scenario,
        IDocumentSession session,
        IProjectionDaemon daemon)
    {
        await daemon.RebuildProjectionAsync<BeheerVerenigingDetailProjection>(CancellationToken.None);

        return await session
                    .Query<BeheerVerenigingDetailDocument>()
                    .SingleAsync(x => x.VCode == scenario.VCode);
    }
}

public class BeheerDetailScenarioClassFixture<TScenario>
    : IClassFixture<BeheerDetailScenarioFixture<TScenario>>
    where TScenario : IScenario, new()
{
}
