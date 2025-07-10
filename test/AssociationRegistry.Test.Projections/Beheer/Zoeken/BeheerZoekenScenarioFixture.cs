namespace AssociationRegistry.Test.Projections.Beheer.Zoeken;

using Admin.ProjectionHost.Projections;
using Admin.Schema.Search;
using Framework.Fixtures;
using JasperFx.Events.Daemon;
using Marten;
using Marten.Events.Daemon;
using Nest;

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
        var getResponse =
            await Context.AdminElasticClient
                         .GetAsync<VerenigingZoekDocument>(scenario.VCode);

        return getResponse.Source;
    }
}

public class BeheerZoekenScenarioClassFixture<TScenario>
    : IClassFixture<BeheerZoekenScenarioFixture<TScenario>>
    where TScenario : IScenario, new()
{
}
