namespace AssociationRegistry.Scheduled.Host.Erkenningen.Activeer;

using Microsoft.Extensions.Logging;
using Quartz;

public class ActiveerErkenningenJob(IErkenningenActivatieProcessor processor, ILogger<ActiveerErkenningenJob> logger)
    : IJob
{
    public const string JobName = "activeer-erkenningen-runner";

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            logger.LogInformation("{ServiceName} started", typeof(ActiveerErkenningenJob));

            await processor.ActiveerErkenningen(context.CancellationToken);

            logger.LogInformation("Automatische activatie van erkenningen werd voltooid.");
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Automatische activatie van erkenningen kon niet voltooid worden. {Message}",
                ex.Message
            );

            throw;
        }
    }
}
