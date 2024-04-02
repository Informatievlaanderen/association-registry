namespace AssociationRegistry.AddressMatch;

using DuplicateVerenigingDetection;
using Framework;
using NodaTime;
using Vereniging;
using IClock = Framework.IClock;

public class TeSynchroniserenAdresMessageHandler
{
    private readonly IClock _clock;
    private readonly IDuplicateVerenigingDetectionService _duplicateVerenigingDetectionService;
    private readonly IVCodeService _vCodeService;
    private readonly IVerenigingsRepository _verenigingsRepository;

    public TeSynchroniserenAdresMessageHandler(
        IVerenigingsRepository verenigingsRepository,
        IVCodeService vCodeService,
        IDuplicateVerenigingDetectionService duplicateVerenigingDetectionService,
        IClock clock)
    {
        _verenigingsRepository = verenigingsRepository;
        _vCodeService = vCodeService;
        _duplicateVerenigingDetectionService = duplicateVerenigingDetectionService;
        _clock = clock;
    }

    public async Task Handle(
        TeSynchroniserenAdresMessage message,
        CancellationToken cancellationToken = default)
    {
        var vereniging = await _verenigingsRepository.Load<Vereniging>(VCode.Hydrate(message.VCode), null);

        // adresmatch

        vereniging.ProbeerAdresTeMatchen();

        await _verenigingsRepository.Save(vereniging, new CommandMetadata("AGV", SystemClock.Instance.GetCurrentInstant(), Guid.NewGuid(), null), CancellationToken.None);
    }
}
