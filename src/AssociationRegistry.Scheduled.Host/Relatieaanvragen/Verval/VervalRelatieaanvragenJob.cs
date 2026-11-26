namespace AssociationRegistry.Scheduled.Host.Relatieaanvragen.Verval;

using Infrastructure.Notifications;
using Integrations.Slack;
using Microsoft.Extensions.Logging;
using Quartz;

/// <summary>
///     Puts a command in the queue for every relatieaanvraag whose vervaldatum is over. A failure is reported in Slack:
///     until the next run, the guards of those relatieaanvragen block a new relatieaanvraag for the same pair.
/// </summary>
public class VervalRelatieaanvragenJob(
    IVervalRelatieaanvragenProcessor processor,
    INotifier notifier,
    ILogger<VervalRelatieaanvragenJob> logger
) : IJob
{
    public const string JobName = "verval-relatieaanvragen-runner";

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            logger.LogInformation("{ServiceName} started", typeof(VervalRelatieaanvragenJob));

            await processor.VervalRelatieaanvragen(context.CancellationToken);

            logger.LogInformation("Automatisch verval van relatieaanvragen: de commands staan in de queue.");
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Automatisch verval van relatieaanvragen kon niet voltooid worden. {Message}",
                ex.Message
            );

            await notifier.Notify(new VervalRelatieaanvragenGefaald(ex));

            throw;
        }
    }
}
