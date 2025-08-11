namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Locaties.ProbeerAdresTeMatchen;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar.Clients;
using Microsoft.Extensions.Logging;

public class ProbeerAdresTeMatchenCommandHandler
{
    private readonly ILogger<ProbeerAdresTeMatchenCommandHandler> _logger;
    private readonly IGrarClient _grarClient;
    private readonly IVerenigingsRepository _verenigingsRepository;

    public ProbeerAdresTeMatchenCommandHandler(
        IVerenigingsRepository verenigingsRepository,
        IGrarClient grarClient,
        ILogger<ProbeerAdresTeMatchenCommandHandler> logger)
    {
        _verenigingsRepository = verenigingsRepository;
        _grarClient = grarClient;
        _logger = logger;
    }

    public async Task Handle(
        ProbeerAdresTeMatchenCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Handle {nameof(ProbeerAdresTeMatchenCommandHandler)}");

        try
        {
            var metadata = CommandMetadata.ForDigitaalVlaanderenProcess;
            var vereniging = await _verenigingsRepository.Load<VerenigingOfAnyKind>(VCode.Hydrate(command.VCode), metadata, allowDubbeleVereniging: true);

            await vereniging.ProbeerAdresTeMatchen(_grarClient, command.LocatieId, cancellationToken);

            await _verenigingsRepository.Save(
                vereniging,
                metadata with { ExpectedVersion = vereniging.Version },
                cancellationToken);
        }
        catch (UnexpectedAggregateVersionException)
        {
            throw new UnexpectedAggregateVersionDuringSyncException();
        }
        catch (VerenigingIsVerwijderd)
        {
            _logger.LogWarning("Kon de locatie niet adresmatchen wegens verwijderde vereniging met VCode: {VCode}.", command.VCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }
}
