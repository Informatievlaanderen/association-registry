namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Locaties.ProbeerAdresTeMatchen;

using System;
using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar.AdresMatch;
using MartenDb.Store;
using Microsoft.Extensions.Logging;

public class ProbeerAdresTeMatchenCommandHandler
{
    private readonly ILogger<ProbeerAdresTeMatchenCommandHandler> _logger;
    private readonly IAdresMatchService _adresMatchService;
    private readonly IAggregateSession _aggregateSession;

    public ProbeerAdresTeMatchenCommandHandler(
        IAggregateSession aggregateSession,
        IAdresMatchService adresMatchService,
        ILogger<ProbeerAdresTeMatchenCommandHandler> logger
    )
    {
        _aggregateSession = aggregateSession;
        _adresMatchService = adresMatchService;
        _logger = logger;
    }

    public async Task Handle(ProbeerAdresTeMatchenCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Handle {nameof(ProbeerAdresTeMatchenCommandHandler)}");

        try
        {
            var metadata = CommandMetadata.ForDigitaalVlaanderenProcess;
            var vereniging = await _aggregateSession.Load<VerenigingOfAnyKind>(
                VCode.Hydrate(command.VCode),
                metadata,
                allowDubbeleVereniging: true
            );

            var request = vereniging.CreateAdresMatchRequest(command.LocatieId);
            var result = await _adresMatchService.ProcessAdresMatch(request, cancellationToken);
            vereniging.ProcessAdresMatchResult(result, command.LocatieId);

            await _aggregateSession.Save(
                vereniging,
                metadata with
                {
                    ExpectedVersion = vereniging.Version,
                },
                cancellationToken
            );
        }
        catch (UnexpectedAggregateVersionException)
        {
            throw new UnexpectedAggregateVersionDuringSyncException();
        }
        catch (VerenigingIsVerwijderd)
        {
            _logger.LogWarning(
                "Kon de locatie niet adresmatchen wegens verwijderde vereniging met VCode: {VCode}.",
                command.VCode
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }
}
