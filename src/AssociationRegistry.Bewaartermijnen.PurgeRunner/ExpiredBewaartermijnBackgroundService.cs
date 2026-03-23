namespace AssociationRegistry.Bewaartermijnen.PurgeRunner;

using Infrastructure.Notifications;
using Integrations.Slack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class ExpiredBewaartermijnBackgroundService(
    IServiceProvider serviceProvider,
    ILogger<ExpiredBewaartermijnBackgroundService> logger,
    IHostApplicationLifetime hostApplicationLifetime
)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        var processor = scope.ServiceProvider.GetRequiredService<IVerlopenBewaartermijnenProcessor>();
        var notifier = scope.ServiceProvider.GetRequiredService<INotifier>();

        try
        {
            logger.LogInformation("{ServiceName} started", typeof(ExpiredBewaartermijnBackgroundService));

            await processor.SendVerlopenBewaartermijnen(cancellationToken);

            logger.LogInformation($"Vervallen bewaartermijnen verwijderen werd voltooid.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Vervallen bewaartermijnen verwijderen kon niet voltooid worden. {ex.Message}");
            await notifier.Notify(new ExpiredBewaartermijnProcessorGefaald(ex));

            throw;
        }
        finally
        {
            hostApplicationLifetime.StopApplication();
        }
    }
}
