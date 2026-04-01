namespace AssociationRegistry.Admin.AddressSync.Handlers;

using CommandHandling.DecentraalBeheer.Acties.Locaties.ProbeerAdresTeMatchen;
using Fetchers;
using Infrastructure.Notifications;
using Integrations.Slack;
using Marten;
using Microsoft.Extensions.Logging;

public class SyncLocatieZonderAdresMatchHandler(
    ILogger<SyncLocatieZonderAdresMatchHandler> logger,
    ITeSynchroniserenLocatiesZonderAdresMatchFetcher fetcher,
    IProbeerAdresTeMatchenCommandHandler probeerAdresTeMatchenCommandHandler
) : ISyncLocatieZonderAdresMatchHandler
{
    public async Task<AdressSyncError[]> Handle(IDocumentSession session, CancellationToken cancellationToken)
    {
        AdressSyncError[] errors = [];

        logger.LogInformation("Locatie zonder adresmatch synchroniseren werd gestart.");

        var locatieZonderAdresMatchDocuments = await fetcher.GetTeSynchroniserenLocatiesZonderAdresMatch(
            session,
            cancellationToken
        );

        logger.LogInformation(
            "Er werden {MessageCount} synchroniseer berichten voor locaties zonder adresmatch gevonden.",
            locatieZonderAdresMatchDocuments.Count()
        );

        foreach (var doc in locatieZonderAdresMatchDocuments)
        {
            foreach (var locatieId in doc.LocatieIds)
            {
                try
                {
                    await probeerAdresTeMatchenCommandHandler.Handle(
                        new ProbeerAdresTeMatchenCommand(doc.VCode, locatieId),
                        cancellationToken
                    );
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        $"Locatie zonder adresmatch synchroniseren kon niet voltooid worden. {ex.Message}"
                    );

                    errors = errors.Append(new AdressSyncError(doc.VCode, [locatieId], ex)).ToArray();
                }
            }
        }

        logger.LogInformation("Locatie zonder adresmatch synchroniseren werd voltooid.");

        return errors;
    }
}

public interface ISyncLocatieZonderAdresMatchHandler
{
    Task<AdressSyncError[]> Handle(IDocumentSession session, CancellationToken cancellationToken);
}
