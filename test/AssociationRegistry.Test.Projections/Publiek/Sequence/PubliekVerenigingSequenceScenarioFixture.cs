namespace AssociationRegistry.Test.Projections.Publiek.Sequence;

using AssociationRegistry.Public.ProjectionHost.Projections;
using Framework.Fixtures;
using JasperFx.Events.Daemon;
using Marten;
using Public.Schema.Sequence;

public class PubliekVerenigingSequenceScenarioFixture<TScenario>(ProjectionContext context)
    : ScenarioFixture<TScenario, PubliekVerenigingSequenceDocument, ProjectionContext>(context)
    where TScenario : IScenario, new()
{
    protected override IDocumentStore DocumentStore => Context.PublicStore;

    protected override async Task RefreshProjectionsAsync(IProjectionDaemon daemon)
    {
        await daemon.RebuildProjectionAsync(ProjectionNames.PubliekSequence, CancellationToken.None);
    }

    protected override async Task<PubliekVerenigingSequenceDocument> GetResultAsync(
        IDocumentSession session,
        TScenario scenario)
        => await session
                .Query<PubliekVerenigingSequenceDocument>()
                .SingleAsync(x => x.VCode == scenario.AggregateId);
}

public class PubliekVerenigingSequenceScenarioClassFixture<TScenario>
    : IClassFixture<PubliekVerenigingSequenceScenarioFixture<TScenario>>
    where TScenario : IScenario, new()
{
}
