namespace AssociationRegistry.Scheduled.Host.Erkenningen;

using Microsoft.Extensions.Logging;
using Quartz;

public class ErkenningenActivatieJob(IErkenningenActivatieProcessor processor, ILogger<ErkenningenActivatieJob> logger)
    : IJob
{
    public const string JobName = "erkenningen-activatie-runner";

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            logger.LogInformation("{ServiceName} started", typeof(ErkenningenActivatieJob));

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
