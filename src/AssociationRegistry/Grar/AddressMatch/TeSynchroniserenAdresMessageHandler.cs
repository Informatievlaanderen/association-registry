namespace AssociationRegistry.Grar.AddressMatch;

using DuplicateVerenigingDetection;
using Framework;
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
            var vereniging = await _verenigingsRepository.Load<Vereniging>(VCode.Hydrate(message.VCode), null);

            await vereniging.ProbeerAdresTeMatchen(_grarClient, message.LocatieId);

            await _verenigingsRepository.Save(
                vereniging, new CommandMetadata("AGV", SystemClock.Instance.GetCurrentInstant(), Guid.NewGuid(), null),
                CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }
    }
}
