namespace AssociationRegistry.Acm.Api.Infrastructure.Metrics;

using Marten;
using Marten.Services;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        var highWatermark = commit.GetEvents().Max(x => x.Sequence);
        _instrumentation.VerenigingPerInszHistogram.Record(highWatermark);
    }
}
