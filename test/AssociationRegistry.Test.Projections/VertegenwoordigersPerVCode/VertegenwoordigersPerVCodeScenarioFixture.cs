namespace AssociationRegistry.Test.Projections.VertegenwoordigersPerVCode;

using Admin.ProjectionHost.Projections.Vertegenwoordiger;
using Admin.Schema.Vertegenwoordiger;
using Framework.Fixtures;
using JasperFx.Events.Daemon;
using Marten;

public class VertegenwoordigersPerVCodeScenarioFixture<TScenario>(ProjectionContext context)
    : ScenarioFixture<TScenario, VertegenwoordigersPerVCodeDocument?, ProjectionContext>(context)
    where TScenario : IScenario, new()
{
    protected override IDocumentStore DocumentStore => Context.AdminStore;

    protected override async Task RefreshProjectionsAsync(IProjectionDaemon daemon)
        => await daemon.RebuildProjectionAsync<VertegenwoordigerProjection>(CancellationToken.None);

    protected override async Task<VertegenwoordigersPerVCodeDocument?> GetResultAsync(
        IDocumentSession session,
        TScenario scenario)
        => await session
                .Query<VertegenwoordigersPerVCodeDocument>()
                .SingleOrDefaultAsync(x => x.VCode == scenario.AggregateId);
}

public class VertegenwoordigersPerVCodeScenarioClassFixture<TScenario>
    : IClassFixture<VertegenwoordigersPerVCodeScenarioFixture<TScenario>>
    where TScenario : IScenario, new()
{
}
