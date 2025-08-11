namespace AssociationRegistry.Admin.AddressSync;

using Grar.Models;
using Infrastructure.Notifications;
using Integrations.Slack;
using Marten;
using MessageHandling.Sqs.AddressSync;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public record NachtelijkeAdresSyncVolgensAdresId(string AdresId, List<LocatieIdWithVCode> LocatieIdWithVCodes);
public record NachtelijkeAdresSyncVolgensVCode(string VCode, List<LocatieWithAdres> LocatieWithAdres);
public record LocatieIdWithVCode(int LocatieId, string VCode);

public class AddressSyncService(
    IServiceProvider serviceProvider,
    ILogger<AddressSyncService> logger,
    IHostApplicationLifetime hostApplicationLifetime
    )
    : BackgroundService
{

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();

        var store = scope.ServiceProvider.GetRequiredService<IDocumentStore>();
        var handler = scope.ServiceProvider.GetRequiredService<TeSynchroniserenLocatieAdresMessageHandler>();
        var teSynchroniserenLocatiesFetcher = scope.ServiceProvider.GetRequiredService<ITeSynchroniserenLocatiesFetcher>();
        var notifier = scope.ServiceProvider.GetRequiredService<INotifier>();

        await using var session = store.LightweightSession();

        try
        {
            logger.LogInformation($"Adressen synchroniseren werd gestart.");

            var messages = await teSynchroniserenLocatiesFetcher.GetTeSynchroniserenLocaties(session, cancellationToken);

            logger.LogInformation($"Er werden {messages.Count()} synchroniseer berichten gevonden.");

            foreach (var synchroniseerLocatieMessage in messages)
            {
                await handler.Handle(synchroniseerLocatieMessage, cancellationToken);
            }

            logger.LogInformation($"Adressen synchroniseren werd voltooid.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Adressen synchroniseren kon niet voltooid worden. {ex.Message}");
            await notifier.Notify(new AdresSynchronisatieGefaald(ex));

            throw;
        }
        finally
        {
            await session.DisposeAsync();
            hostApplicationLifetime.StopApplication();
        }
    }
}
