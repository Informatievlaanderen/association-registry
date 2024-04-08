namespace AssociationRegistry.Acties.RegistreerFeitelijkeVereniging;

using DuplicateVerenigingDetection;
using Events;
using Framework;
using Grar.AddressMatch;
using Marten;
using Microsoft.Extensions.Logging;
using ResultNet;
using Vereniging;
using Wolverine.Marten;

public class RegistreerFeitelijkeVerenigingCommandHandler
{
    private readonly IClock _clock;
    private readonly ILogger<RegistreerFeitelijkeVerenigingCommandHandler> _logger;
    private readonly IDuplicateVerenigingDetectionService _duplicateVerenigingDetectionService;
    private readonly IMartenOutbox _outbox;
    private readonly IDocumentSession _session;
    private readonly IVCodeService _vCodeService;
    private readonly IVerenigingsRepository _verenigingsRepository;

    public RegistreerFeitelijkeVerenigingCommandHandler(
        IVerenigingsRepository verenigingsRepository,
        IVCodeService vCodeService,
        IDuplicateVerenigingDetectionService duplicateVerenigingDetectionService,
        IMartenOutbox outbox,
        IDocumentSession session,
        IClock clock,
        ILogger<RegistreerFeitelijkeVerenigingCommandHandler> logger)
    {
        _verenigingsRepository = verenigingsRepository;
        _vCodeService = vCodeService;
        _duplicateVerenigingDetectionService = duplicateVerenigingDetectionService;
        _outbox = outbox;
        _session = session;
        _clock = clock;
        _logger = logger;
    }

    public async Task<Result> Handle(
        CommandEnvelope<RegistreerFeitelijkeVerenigingCommand> message,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handle RegistreerFeitelijkeVerenigingCommandHandler start");

        var command = message.Command;

        if (!command.SkipDuplicateDetection)
        {
            var duplicates = (await _duplicateVerenigingDetectionService.GetDuplicates(command.Naam, command.Locaties)).ToList();

            if (duplicates.Any())
                return new Result<PotentialDuplicatesFound>(new PotentialDuplicatesFound(duplicates), ResultStatus.Failed);
        }

        var vCode = await _vCodeService.GetNext();

        var vereniging = Vereniging.RegistreerFeitelijkeVereniging(
            vCode,
            command.Naam,
            command.KorteNaam,
            command.KorteBeschrijving,
            command.Startdatum,
            command.Doelgroep,
            command.IsUitgeschrevenUitPubliekeDatastroom,
            command.Contactgegevens,
            command.Locaties,
            message.Command.Vertegenwoordigers,
            command.HoofdactiviteitenVerenigingsloket,
            _clock);

        var toegevoegdeLocaties = vereniging.UncommittedEvents.OfType<FeitelijkeVerenigingWerdGeregistreerd>()
                                            .Single().Locaties;

        foreach (var teSynchroniserenLocatie in toegevoegdeLocaties)
        {
            if (teSynchroniserenLocatie.Adres is not null)
            {
                await _outbox.SendAsync(new TeSynchroniserenAdresMessage(vCode.Value, teSynchroniserenLocatie.LocatieId));
            }
        }

        var result = await _verenigingsRepository.Save(vereniging, _session ,message.Metadata, cancellationToken);

        _logger.LogInformation("Handle RegistreerFeitelijkeVerenigingCommandHandler end");

        return Result.Success(CommandResult.Create(vCode, result));
    }
}
