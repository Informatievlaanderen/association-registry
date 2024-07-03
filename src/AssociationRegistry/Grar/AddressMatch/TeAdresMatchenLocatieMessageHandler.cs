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
        TeAdresMatchenLocatieMessage matchenLocatieMessage,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Handle {nameof(TeAdresMatchenLocatieMessageHandler)}");

        try
        {
            var vereniging = await _verenigingsRepository.Load<VerenigingOfAnyKind>(VCode.Hydrate(matchenLocatieMessage.VCode));

            await vereniging.ProbeerAdresTeMatchen(_grarClient, matchenLocatieMessage.LocatieId, cancellationToken);

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
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }
}
