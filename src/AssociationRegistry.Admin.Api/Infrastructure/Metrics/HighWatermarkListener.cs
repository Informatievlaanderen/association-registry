namespace AssociationRegistry.Admin.Api.Infrastructure.Metrics;

using global::Marten;
using global::Marten.Services;
using Marten;
using Marten.Services;

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
