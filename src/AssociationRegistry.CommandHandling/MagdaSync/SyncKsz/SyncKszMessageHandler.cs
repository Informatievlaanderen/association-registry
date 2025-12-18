namespace AssociationRegistry.CommandHandling.MagdaSync.SyncKsz;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using Framework;
using Microsoft.Extensions.Logging;
using Persoonsgegevens;
using Wolverine;

public class SyncKszMessageHandler
{
    private readonly IVertegenwoordigerPersoonsgegevensRepository _vertegenwoordigerPersoonsgegevensRepository;
    private readonly IVerenigingsRepository _verenigingsRepository;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<SyncKszMessageHandler> _logger;

    public SyncKszMessageHandler(
        IVertegenwoordigerPersoonsgegevensRepository vertegenwoordigerPersoonsgegevensRepository,
        IVerenigingsRepository verenigingsRepository,
        ILogger<SyncKszMessageHandler> logger)
    {
        _vertegenwoordigerPersoonsgegevensRepository = vertegenwoordigerPersoonsgegevensRepository;
        _verenigingsRepository = verenigingsRepository;
        _logger = logger;
    }

    public async Task Handle(
        SyncKszMessage message,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("SyncKszMessageHandler started");

        if (!message.Overleden) // only need to sync if overleden for now
            return;

        var vertegenwoordigerPersoonsgegevens = await _vertegenwoordigerPersoonsgegevensRepository.Get(message.Insz, cancellationToken);

        if (!vertegenwoordigerPersoonsgegevens.Any())
            return;

        // TODO: uitschrijven or-2939

        var commandMetadata = CommandMetadata.ForDigitaalVlaanderenProcess;

        foreach (var vertegenwoordigerPersoonsgegeven in vertegenwoordigerPersoonsgegevens.DistinctBy(x => x.VCode))
        {
            var vereniging =
                await _verenigingsRepository.Load<Vereniging>(VCode.Create(vertegenwoordigerPersoonsgegeven.VCode), commandMetadata, allowDubbeleVereniging: true, allowVerwijderdeVereniging: true);

            vereniging.MarkeerVertegenwoordigerAlsOverleden(vertegenwoordigerPersoonsgegeven.VertegenwoordigerId);

            await _verenigingsRepository.Save(vereniging, commandMetadata, cancellationToken);

            _logger.LogInformation($"SyncKszMessageHandler synced with vCode: {vertegenwoordigerPersoonsgegeven.VCode} for verrtegenwoordiger: {vertegenwoordigerPersoonsgegeven.VertegenwoordigerId}");
        }

        _logger.LogInformation("SyncKszMessageHandler done");
    }
}
