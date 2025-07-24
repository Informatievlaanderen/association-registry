namespace AssociationRegistry.DecentraalBeheer.Administratie.ProbeerAdresTeMatchen;

using EventStore;
using Framework;
using Grar.Clients;
using Locaties.ProbeerAdresTeMatchen;
using Microsoft.Extensions.Logging;
using Vereniging;
using Vereniging.Exceptions;
using Vereniging.Geotags;

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
