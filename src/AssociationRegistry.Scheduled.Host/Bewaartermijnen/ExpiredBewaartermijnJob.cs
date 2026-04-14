namespace AssociationRegistry.Scheduled.Host.Bewaartermijnen;

using AssociationRegistry.Integrations.Slack;
using AssociationRegistry.Scheduled.Host.Infrastructure.Notifications;
using Microsoft.Extensions.Logging;
using Quartz;

public class ExpiredBewaartermijnJob(
    IVerlopenBewaartermijnenProcessor processor,
    INotifier notifier,
    ILogger<ExpiredBewaartermijnJob> logger
) : IJob
{
    public const string JobName = "bewaartermijn-purge-runner";

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            logger.LogInformation("{ServiceName} started", typeof(ExpiredBewaartermijnJob));

            await processor.SendVerlopenBewaartermijnen(CancellationToken.None);

            logger.LogInformation($"Vervallen bewaartermijnen verwijderen werd voltooid.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Vervallen bewaartermijnen verwijderen kon niet voltooid worden. {ex.Message}");
            await notifier.Notify(new ExpiredBewaartermijnProcessorGefaald(ex));

            throw;
        }
    }
}
