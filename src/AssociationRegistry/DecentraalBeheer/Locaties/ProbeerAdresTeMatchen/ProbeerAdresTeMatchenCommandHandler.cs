namespace AssociationRegistry.DecentraalBeheer.Administratie.ProbeerAdresTeMatchen;

using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using Grar.Clients;
using Locaties.ProbeerAdresTeMatchen;
using Microsoft.Extensions.Logging;
using Vereniging.Geotags;

public class ProbeerAdresTeMatchenCommandHandler
{
    private readonly ILogger<ProbeerAdresTeMatchenCommandHandler> _logger;
    private readonly IGrarClient _grarClient;
    private readonly IVerenigingsRepository _verenigingsRepository;
    private readonly IGeotagsService _geotagsService;

    public ProbeerAdresTeMatchenCommandHandler(
        IVerenigingsRepository verenigingsRepository,
        IGrarClient grarClient,
        ILogger<ProbeerAdresTeMatchenCommandHandler> logger,
        IGeotagsService geotagsService)
    {
        _verenigingsRepository = verenigingsRepository;
        _grarClient = grarClient;
        _logger = logger;
        _geotagsService = geotagsService;
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

            await vereniging.HerberekenGeotags(_geotagsService);

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
