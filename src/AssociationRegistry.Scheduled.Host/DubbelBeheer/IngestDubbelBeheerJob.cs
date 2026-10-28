namespace AssociationRegistry.Scheduled.Host.DubbelBeheer;

using AssociationRegistry.Integrations.Slack;
using AssociationRegistry.Scheduled.Host.Infrastructure.Notifications;
using Microsoft.Extensions.Logging;
using Quartz;

/// <summary>
///     Leest het CSV-bestand met dubbele verenigingen van S3 in.
///     Wordt enkel via een HTTP-trigger vanuit de Admin.Api gestart.
/// </summary>
[DisallowConcurrentExecution]
public class IngestDubbelBeheerJob(
    DubbelImportIngestor ingestor,
    INotifier notifier,
    ILogger<IngestDubbelBeheerJob> logger
) : IJob
{
    public const string JobName = "ingest-dubbelbeheer-runner";

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            logger.LogInformation("{ServiceName} started", typeof(IngestDubbelBeheerJob));

            await ingestor.Ingest(context.CancellationToken);

            logger.LogInformation("Inlezen van dubbele verenigingen werd voltooid.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Inlezen van dubbele verenigingen kon niet voltooid worden. {Message}", ex.Message);
            await notifier.Notify(new IngestDubbelBeheerGefaald(ex));

            throw;
        }
    }
}
