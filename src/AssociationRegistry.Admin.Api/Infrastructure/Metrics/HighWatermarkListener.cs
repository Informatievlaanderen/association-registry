namespace AssociationRegistry.Admin.Api.Infrastructure.Metrics;

using Marten;
using Marten.Services;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class HighWatermarkListener : DocumentSessionListenerBase
{
    private readonly Instrumentation _instrumentation;

    public HighWatermarkListener(Instrumentation instrumentation)
    {
        _instrumentation = instrumentation;
    }

    public override async Task AfterCommitAsync(IDocumentSession session, IChangeSet commit, CancellationToken token)
    {
        var events = commit.GetEvents().ToArray();

        if (events.Any())
        {
            var highWatermark = events.Max(x => x.Sequence);
            _instrumentation.HighWatermarkEventValue = highWatermark;
        }
    }
}
