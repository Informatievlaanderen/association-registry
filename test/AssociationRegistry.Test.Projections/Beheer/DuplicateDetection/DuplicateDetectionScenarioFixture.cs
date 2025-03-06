namespace AssociationRegistry.Test.Projections.Beheer.DuplicateDetection;

using AssociationRegistry.Admin.ProjectionHost.Projections;
using Admin.Schema.Search;
using Framework.Fixtures;
using Marten;
using Marten.Events.Daemon;
using Nest;
using System.Threading;
using System.Threading.Tasks;

public class DuplicateDetectionScenarioFixture<TScenario>(ProjectionContext context)
    : ScenarioFixture<TScenario, DuplicateDetectionDocument, ProjectionContext>(context)
    where TScenario : IScenario, new()
{
    protected override IDocumentStore DocumentStore => Context.AdminStore;

    protected override async Task RefreshProjectionsAsync(IProjectionDaemon daemon)
    {
        await daemon.RebuildProjectionAsync(ProjectionNames.DuplicateDetection, CancellationToken.None);
        await Context.AdminElasticClient.Indices.RefreshAsync(Indices.All);
    }

    protected override async Task<DuplicateDetectionDocument> GetResultAsync(
        IDocumentSession _,
        TScenario scenario)
    {
        var getResponse =
            await Context.AdminElasticClient
                         .GetAsync<DuplicateDetectionDocument>(scenario.VCode);

        return getResponse.Source;
    }
}

public class DuplicateDetectionClassFixture<TScenario>
    : IClassFixture<DuplicateDetectionScenarioFixture<TScenario>>
    where TScenario : IScenario, new()
{
}
