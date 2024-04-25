namespace AssociationRegistry.Grar.AddressMatch;

using DuplicateVerenigingDetection;
using EventStore;
using Framework;
using Marten;
using Microsoft.Extensions.Logging;
using NodaTime;
using Vereniging;
using IClock = Framework.IClock;

public class TeSynchroniserenAdresMessageHandler
{
    private readonly IClock _clock;
    private readonly ILogger<TeSynchroniserenAdresMessageHandler> _logger;
    private readonly IDuplicateVerenigingDetectionService _duplicateVerenigingDetectionService;
    private readonly IGrarClient _grarClient;
    private readonly IVCodeService _vCodeService;
    private readonly IDocumentSession _documentSession;
    private readonly IVerenigingsRepository _verenigingsRepository;

    public TeSynchroniserenAdresMessageHandler(
        IVerenigingsRepository verenigingsRepository,
        IVCodeService vCodeService,
        IDuplicateVerenigingDetectionService duplicateVerenigingDetectionService,
        IGrarClient grarClient,
        IClock clock,
        ILogger<TeSynchroniserenAdresMessageHandler> logger)
    {
        _verenigingsRepository = verenigingsRepository;
        _vCodeService = vCodeService;
        _duplicateVerenigingDetectionService = duplicateVerenigingDetectionService;
        _grarClient = grarClient;
        _clock = clock;
        _logger = logger;
    }

    public async Task Handle(
        TeSynchroniserenAdresMessage message,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handle TeSynchroniserenAdresMessageHandler");

        try
        {
            var vereniging = await _verenigingsRepository.Load<VerenigingOfAnyKind>(VCode.Hydrate(message.VCode), null);

            await vereniging.ProbeerAdresTeMatchen(_grarClient, message.LocatieId);

            await _verenigingsRepository.Save(
                vereniging,
                new CommandMetadata(EventStore.DigitaalVlaanderenOvoNumber, SystemClock.Instance.GetCurrentInstant(), Guid.NewGuid(),
                                    vereniging.Version),
                CancellationToken.None);
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
