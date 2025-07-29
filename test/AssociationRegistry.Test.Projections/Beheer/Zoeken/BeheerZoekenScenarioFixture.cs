namespace AssociationRegistry.Test.Projections.Beheer.Zoeken;

using Admin.ProjectionHost.Projections;
using Admin.Schema.Search;
using Framework.Fixtures;
using JasperFx.Events.Daemon;
using Marten;
using Nest;
using Polly;

public class BeheerZoekenScenarioFixture<TScenario>(ProjectionContext context)
    : ScenarioFixture<TScenario, VerenigingZoekDocument, ProjectionContext>(context)
    where TScenario : IScenario, new()
{
    protected override IDocumentStore DocumentStore => Context.AdminStore;

    protected override async Task RefreshProjectionsAsync(IProjectionDaemon daemon)
    {
        await daemon.RebuildProjectionAsync(ProjectionNames.BeheerZoek, CancellationToken.None);
        await Context.AdminElasticClient.Indices.RefreshAsync(Indices.All);
    }

    protected override async Task<VerenigingZoekDocument> GetResultAsync(
        IDocumentSession _,
        TScenario scenario)
    {
        var retryPolicy = Polly.Policy
                               .HandleResult<GetResponse<VerenigingZoekDocument>>(result =>
                                                                                      result == null || !result.Found)
                               .WaitAndRetryAsync(
                                    retryCount: 3,
                                    sleepDurationProvider: retryAttempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, retryAttempt))); // Exponential backoff: 200ms, 400ms, 800ms

        var getResponse = await retryPolicy.ExecuteAsync(async () =>
        {
            return await Context.AdminElasticClient
                                .GetAsync<VerenigingZoekDocument>(scenario.VCode);
        });

        return getResponse?.Source;
    }
}

public class BeheerZoekenScenarioClassFixture<TScenario>
    : IClassFixture<BeheerZoekenScenarioFixture<TScenario>>
    where TScenario : IScenario, new()
{
}
