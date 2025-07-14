namespace AssociationRegistry.Test.Projections.Acm;

using AssociationRegistry.Acm.Api.Projections;
using AssociationRegistry.Acm.Schema.VerenigingenPerInsz;
using Framework.Fixtures;
using JasperFx.Events.Daemon;
using Marten;
using Marten.Events.Daemon;

public class VerenigingenPerInszScenarioFixture<TScenario>(ProjectionContext context)
    : ScenarioFixture<TScenario, VerenigingenPerInszDocument, ProjectionContext>(context)
    where TScenario : InszScenarioBase, new()
{
    protected override IDocumentStore DocumentStore => Context.AcmStore;

    protected override async Task RefreshProjectionsAsync(IProjectionDaemon daemon)
        => await daemon.RebuildProjectionAsync<VerenigingenPerInszProjection>(CancellationToken.None);

    protected override async Task<VerenigingenPerInszDocument> GetResultAsync(
        IDocumentSession session,
        TScenario scenario)
    {
        var docs = await session
                        .Query<VerenigingenPerInszDocument>().ToListAsync();
        return await session
                    .Query<VerenigingenPerInszDocument>()
                    .SingleAsync(x => x.Insz == scenario.Insz);
    }
}

public class VerenigingenPerInszScenarioClassFixture<TScenario>
    : IClassFixture<VerenigingenPerInszScenarioFixture<TScenario>>
    where TScenario : InszScenarioBase, new()
{
}
