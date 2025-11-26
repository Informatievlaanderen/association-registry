namespace AssociationRegistry.Test.Projections.Beheer.Historiek;

using Admin.ProjectionHost.Projections.Historiek;
using Admin.Schema.Historiek;
using Framework.Fixtures;
using JasperFx.Events.Daemon;
using Marten;

public class BeheerHistoriekScenarioFixture<TScenario>(ProjectionContext context)
    : ScenarioFixture<TScenario, BeheerVerenigingHistoriekDocument, ProjectionContext>(context)
    where TScenario : IScenario, new()
{
    protected override IDocumentStore DocumentStore => Context.AdminStore;

    protected override async Task RefreshProjectionsAsync(IProjectionDaemon daemon)
        => await daemon.RebuildProjectionAsync<BeheerVerenigingHistoriekProjection>(CancellationToken.None);

    protected override async Task<BeheerVerenigingHistoriekDocument> GetResultAsync(
        IDocumentSession session,
        TScenario scenario)
        => await session
                .Query<BeheerVerenigingHistoriekDocument>()
                .SingleAsync(x => x.VCode == scenario.AggregateId);
}

public class BeheerHistoriekScenarioClassFixture<TScenario>
    : IClassFixture<BeheerHistoriekScenarioFixture<TScenario>>
    where TScenario : IScenario, new()
{
}
