namespace AssociationRegistry.DecentraalBeheer.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;

using DuplicateVerenigingDetection;
using Events;
using Framework;
using Grar.Clients;
using JasperFx.Core;
using Messages;
using Vereniging;
using Marten;
using Microsoft.Extensions.Logging;
using ResultNet;
using Vereniging.Geotags;
using Wolverine.Marten;

public class RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler
{
    private readonly IClock _clock;
    private readonly IGrarClient _grarClient;
    private readonly IGeotagsService _geotagsService;
    private readonly ILogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler> _logger;
    private readonly IDuplicateVerenigingDetectionService _duplicateVerenigingDetectionService;
    private readonly IMartenOutbox _outbox;
    private readonly IDocumentSession _session;
    private readonly IVCodeService _vCodeService;
    private readonly IVerenigingsRepository _verenigingsRepository;

    public RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler(
        IVerenigingsRepository verenigingsRepository,
        IVCodeService vCodeService,
        IDuplicateVerenigingDetectionService duplicateVerenigingDetectionService,
        IMartenOutbox outbox,
        IDocumentSession session,
        IClock clock,
        IGrarClient grarClient,
        IGeotagsService geotagsService,
        ILogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler> logger)
    {
        _verenigingsRepository = verenigingsRepository;
        _vCodeService = vCodeService;
        _duplicateVerenigingDetectionService = duplicateVerenigingDetectionService;
        _outbox = outbox;
        _session = session;
        _clock = clock;
        _grarClient = grarClient;
        _geotagsService = geotagsService;
        _logger = logger;
    }

    public async Task<Result> Handle(
        CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> message,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Handle {nameof(RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler)} start");

        var command = message.Command;

        if (!command.SkipDuplicateDetection)
        {
            var duplicates = (await _duplicateVerenigingDetectionService.GetDuplicates(command.Naam, command.Locaties)).ToList();

            if (duplicates.Any())
                return new Result<PotentialDuplicatesFound>(new PotentialDuplicatesFound(duplicates), ResultStatus.Failed);
        }


        var vereniging = await Vereniging.RegistreerVerenigingZonderEigenRechtspersoonlijkheid(
            command,
            _vCodeService,
            _geotagsService,
            _clock);

        var (metAdresId, zonderAdresId) = vereniging.GeefLocatiesMetEnZonderAdresId();

        await vereniging.NeemAdresDetailsOver(metAdresId, _grarClient, CancellationToken.None);
        await vereniging.BerekenGeotags(_geotagsService);

        foreach (var locatieZonderAdresId in zonderAdresId)
        {
            await _outbox.SendAsync(new TeAdresMatchenLocatieMessage(vereniging.VCode, locatieZonderAdresId.LocatieId));
        }

        var result = await _verenigingsRepository.SaveNew(vereniging, _session ,message.Metadata, cancellationToken);

        _logger.LogInformation($"Handle {nameof(RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler)} end");

        return Result.Success(CommandResult.Create(vereniging.VCode, result));
    }
}
