namespace AssociationRegistry.Public.ProjectionHost.Infrastructure.Metrics;

using Marten;
using Marten.Events.Daemon;
using Marten.Services;
using Projections.Detail;

public class HighWatermarkListener : IDocumentSessionListener
{
    private readonly Instrumentation _instrumentation;

    public HighWatermarkListener(Instrumentation instrumentation)
    {
        _instrumentation = instrumentation;
    }

    public void BeforeSaveChanges(IDocumentSession session)
    {
    }

    public async Task BeforeSaveChangesAsync(IDocumentSession session, CancellationToken token)
    {
    }

    public void AfterCommit(IDocumentSession session, IChangeSet commit)
    {
    }

    public void DocumentLoaded(object id, object document)
    {
    }

    public void DocumentAddedForStorage(object id, object document)
    {
    }

    public async Task AfterCommitAsync(IDocumentSession session, IChangeSet commit, CancellationToken token)
    {
        var publiekDetailProgress = await session.DocumentStore.Advanced.ProjectionProgressFor(
            new ShardName(typeof(PubliekVerenigingDetailProjection).Namespace),
            token: token);

        _instrumentation.PubliekVerenigingDetailHistogram.Record(publiekDetailProgress);

        var publiekZoekenProgress = await session.DocumentStore.Advanced.ProjectionProgressFor(
            new ShardName("PubliekVerenigingZoekenProjection"),
            token: token);

        _instrumentation.PubliekVerenigingZoekenHistogram.Record(publiekZoekenProgress);
    }
}
