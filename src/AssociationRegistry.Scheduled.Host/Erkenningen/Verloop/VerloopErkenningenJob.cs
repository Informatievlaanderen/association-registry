namespace AssociationRegistry.Scheduled.Host.Erkenningen.Verloop;

using Microsoft.Extensions.Logging;
using Quartz;

public class VerloopErkenningenJob(IVerloopErkenningenProcessor processor, ILogger<VerloopErkenningenJob> logger)
    : IJob
{
    public const string JobName = "verloop-erkenningen-runner";

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            logger.LogInformation("{ServiceName} started", typeof(VerloopErkenningenJob));

            await processor.VerloopErkenningen(context.CancellationToken);

            logger.LogInformation("Automatisch verloop van erkenningen werd voltooid.");
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Automatisch verloop van erkenningen kon niet voltooid worden. {Message}",
                ex.Message
            );

            throw;
        }
    }
}
