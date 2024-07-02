namespace AssociationRegistry.Grar.AddressMatch;

using EventStore;
using Framework;
using Marten;
using Microsoft.Extensions.Logging;
using NodaTime;
using Vereniging;

public class TeSynchroniserenAdresMessageHandler
{
    private readonly ILogger<TeSynchroniserenAdresMessageHandler> _logger;
    private readonly IGrarClient _grarClient;
    private readonly IDocumentSession _documentSession;
    private readonly IVerenigingsRepository _verenigingsRepository;

    public TeSynchroniserenAdresMessageHandler(
        IVerenigingsRepository verenigingsRepository,
        IGrarClient grarClient,
        ILogger<TeSynchroniserenAdresMessageHandler> logger)
    {
        _verenigingsRepository = verenigingsRepository;
        _grarClient = grarClient;
        _logger = logger;
    }

    public async Task Handle(
        TeSynchroniserenAdresMessage message,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handle TeSynchroniserenAdresMessageHandler");

        try
        {
            var vereniging = await _verenigingsRepository.Load<VerenigingOfAnyKind>(VCode.Hydrate(message.VCode));

            await vereniging.ProbeerAdresTeMatchen(_grarClient, message.LocatieId, cancellationToken);

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
