namespace AssociationRegistry.Test.Projections.Publiek.Zoeken;

using Framework.Fixtures;
using JasperFx.Events.Daemon;
using Marten;
using Marten.Events.Daemon;
using Nest;
using Public.ProjectionHost.Projections;
using Public.Schema.Search;

public class PubliekZoekenScenarioFixture<TScenario>(ProjectionContext context)
    : ScenarioFixture<TScenario, VerenigingZoekDocument, ProjectionContext>(context)
    where TScenario : IScenario, new()
{
    protected override IDocumentStore DocumentStore => Context.PublicStore;

    protected override async Task RefreshProjectionsAsync(IProjectionDaemon daemon)
    {
        await daemon.RebuildProjectionAsync(ProjectionNames.PubliekZoek, CancellationToken.None);
        await Context.PublicElasticClient.Indices.RefreshAsync(Indices.All);
    }

    protected override async Task<VerenigingZoekDocument> GetResultAsync(
        IDocumentSession _,
        TScenario scenario)
    {
        var getResponse =
            await Context.PublicElasticClient
                         .GetAsync<VerenigingZoekDocument>(scenario.VCode);

        return getResponse.Source;
    }
}

public class PubliekZoekenScenarioClassFixture<TScenario>
    : IClassFixture<PubliekZoekenScenarioFixture<TScenario>>
    where TScenario : IScenario, new()
{
}
