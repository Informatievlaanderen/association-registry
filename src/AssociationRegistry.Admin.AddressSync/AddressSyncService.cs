namespace AssociationRegistry.Admin.AddressSync;

using Grar.Models;
using Handlers;
using Infrastructure.Notifications;
using Integrations.Slack;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public record NachtelijkeAdresSyncVolgensAdresId(string AdresId, List<LocatieIdWithVCode> LocatieIdWithVCodes);

public record NachtelijkeAdresSyncVolgensVCode(string VCode, List<LocatieWithAdres> LocatieWithAdres);

public record LocatieIdWithVCode(int LocatieId, string VCode);

public class AddressSyncService(
    IHostApplicationLifetime hostApplicationLifetime,
    ILogger<AddressSyncService> logger,
    INotifier notifier,
    IServiceProvider serviceProvider,
    ISyncLocatieAdresHandler syncLocatieAdresHandler,
    ISyncLocatieZonderAdresMatchHandler syncLocatieZonderAdresMatchHandler
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var store = scope.ServiceProvider.GetRequiredService<IDocumentStore>();
        await using var session = store.LightweightSession();
        AdressSyncError[] errors = [];
        try
        {
            var locatieAdresSyncErrors = await syncLocatieAdresHandler.Handle(cancellationToken);
            errors = errors.Union(locatieAdresSyncErrors).ToArray();

            var locatieZonderAdresMatchSyncErrors = await syncLocatieZonderAdresMatchHandler.Handle(
                session,
                cancellationToken
            );
            errors = errors.Union(locatieZonderAdresMatchSyncErrors).ToArray();

            if (errors.Any())
            {
                throw new Exception();
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"AddressSyncService kon niet voltooid worden. {ex.Message}");
            await notifier.Notify(new AdresMatchSynchronisatieGefaald(errors));
            throw;
        }
        finally
        {
            await session.DisposeAsync();
            hostApplicationLifetime.StopApplication();
        }
    }
}

public record AdressSyncError(string VCode, List<int> LocatieIds, Exception Exception);
