namespace AssociationRegistry.Admin.AddressSync.Handlers;

using CommandHandling.DecentraalBeheer.Acties.Locaties.ProbeerAdresTeMatchen;
using Fetchers;
using Marten;
using Microsoft.Extensions.Logging;

public class SyncLocatieZonderAdresMatchProcessor : ISyncLocatieZonderAdresMatchProcessor
{
    private readonly ITeSynchroniserenLocatiesZonderAdresMatchFetcher _fetcher;
    private readonly IProbeerAdresTeMatchenCommandHandler _probeerAdresTeMatchenCommandHandler;
    private readonly IDocumentSession _session;
    private readonly ILogger<SyncLocatieZonderAdresMatchProcessor> _logger;

    public SyncLocatieZonderAdresMatchProcessor(ITeSynchroniserenLocatiesZonderAdresMatchFetcher fetcher,
                                                IProbeerAdresTeMatchenCommandHandler probeerAdresTeMatchenCommandHandler,
                                                IDocumentSession session,
                                                ILogger<SyncLocatieZonderAdresMatchProcessor> logger)
    {
        _fetcher = fetcher;
        _probeerAdresTeMatchenCommandHandler = probeerAdresTeMatchenCommandHandler;
        _session = session;
        _logger = logger;
    }

    public async Task<AdressSyncError[]> Process(CancellationToken cancellationToken)
    {
        AdressSyncError[] errors = [];

        _logger.LogInformation("Locatie zonder adresmatch synchroniseren werd gestart.");

        var locatieZonderAdresMatchDocuments = await _fetcher.GetTeSynchroniserenLocatiesZonderAdresMatch(
            _session,
            cancellationToken
        );

        _logger.LogInformation(
            "Er werden {MessageCount} synchroniseer berichten voor locaties zonder adresmatch gevonden.",
            locatieZonderAdresMatchDocuments.Count()
        );

        foreach (var doc in locatieZonderAdresMatchDocuments)
        {
            foreach (var locatieId in doc.LocatieIds)
            {
                try
                {
                    await _probeerAdresTeMatchenCommandHandler.Handle(
                        new ProbeerAdresTeMatchenCommand(doc.VCode, locatieId),
                        cancellationToken
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        $"Locatie zonder adresmatch synchroniseren kon niet voltooid worden. {ex.Message}"
                    );

                    errors = errors.Append(new AdressSyncError(doc.VCode, [locatieId], ex)).ToArray();
                }
            }
        }

        _logger.LogInformation("Locatie zonder adresmatch synchroniseren werd voltooid.");

        return errors;
    }
}

public interface ISyncLocatieZonderAdresMatchProcessor
{
    Task<AdressSyncError[]> Process(CancellationToken cancellationToken);
}
