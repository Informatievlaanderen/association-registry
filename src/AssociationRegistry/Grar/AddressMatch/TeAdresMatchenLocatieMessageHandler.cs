namespace AssociationRegistry.Grar.AddressMatch;

using EventStore;
using Framework;
using Microsoft.Extensions.Logging;
using NodaTime;
using Vereniging;

public class TeAdresMatchenLocatieMessageHandler
{
    private readonly ILogger<TeAdresMatchenLocatieMessageHandler> _logger;
    private readonly IGrarClient _grarClient;
    private readonly IVerenigingsRepository _verenigingsRepository;

    public TeAdresMatchenLocatieMessageHandler(
        IVerenigingsRepository verenigingsRepository,
        IGrarClient grarClient,
        ILogger<TeAdresMatchenLocatieMessageHandler> logger)
    {
        _verenigingsRepository = verenigingsRepository;
        _grarClient = grarClient;
        _logger = logger;
    }

    public async Task Handle(
        TeAdresMatchenLocatieMessage teAdresMatchenLocatieMessage,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Handle {nameof(TeAdresMatchenLocatieMessageHandler)}");

        try
        {
            var vereniging = await _verenigingsRepository.Load<VerenigingOfAnyKind>(VCode.Hydrate(teAdresMatchenLocatieMessage.VCode), allowDubbeleVereniging: true);

            await vereniging.ProbeerAdresTeMatchen(_grarClient, teAdresMatchenLocatieMessage.LocatieId, cancellationToken);

            await _verenigingsRepository.Save(
                vereniging,
                new CommandMetadata(EventStore.DigitaalVlaanderenOvoNumber, SystemClock.Instance.GetCurrentInstant(), Guid.NewGuid(),
                                    vereniging.Version),
                cancellationToken);
        }
        catch (UnexpectedAggregateVersionException)
        {
            throw new UnexpectedAggregateVersionDuringSyncException();
        }
        catch (AssociationRegistry.Vereniging.Exceptions.VerenigingIsVerwijderd)
        {
            _logger.LogWarning("Kon de locatie niet adresmatchen wegens verwijderde vereniging met VCode: {VCode}.", teAdresMatchenLocatieMessage.VCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }
}
