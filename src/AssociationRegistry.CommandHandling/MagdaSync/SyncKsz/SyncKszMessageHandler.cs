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
        if (!message.Overleden) // only need to sync if overleden for now
        {
            _logger.LogInformation("Skipping message because this person is not deceased");

            return;
        }

        var vertegenwoordigerPersoonsgegevens = await _vertegenwoordigerPersoonsgegevensRepository.Get(message.Insz, cancellationToken);

        if (!vertegenwoordigerPersoonsgegevens.Any())
        {
            // TODO: uitschrijven or-2939

            _logger.LogWarning("Skipping message because this person did not match any known VertegenwoordigerPersoonsgegevensDocument");

            return;
        }

        var commandMetadata = CommandMetadata.ForDigitaalVlaanderenProcess;

        var vzerOnly = await FilterOnlyVzer(vertegenwoordigerPersoonsgegevens);

        if (!vzerOnly.Any())
        {
            _logger.LogInformation("Only found kbo associations for this person");
        }


        foreach (var vertegenwoordigerPersoonsgegeven in vzerOnly)
        {
            var vereniging =
                await _verenigingsRepository.Load<Vereniging>(VCode.Create(vertegenwoordigerPersoonsgegeven.VCode), commandMetadata, allowDubbeleVereniging: true, allowVerwijderdeVereniging: true);

            vereniging.MarkeerVertegenwoordigerAlsOverleden(vertegenwoordigerPersoonsgegeven.VertegenwoordigerId);

            await _verenigingsRepository.Save(vereniging, commandMetadata, cancellationToken);

            _logger.LogInformation($"SyncKszMessageHandler marked vertegenwoordiger as deceased with vCode: {vertegenwoordigerPersoonsgegeven.VCode} for vertegenwoordiger: {vertegenwoordigerPersoonsgegeven.VertegenwoordigerId}");
        }

        _logger.LogInformation("SyncKszMessageHandler done");
    }

    private async Task<List<VertegenwoordigerPersoonsgegevens>> FilterOnlyVzer(VertegenwoordigerPersoonsgegevens[] vertegenwoordigerPersoonsgegevens)
    {
        var vertegenwoordigerPersoonsgegevensByVCode = vertegenwoordigerPersoonsgegevens.DistinctBy(x => x.VCode).ToList();
        var vzerOnlyVcodes = await _verenigingsRepository.FilterVzerOnly(vertegenwoordigerPersoonsgegevensByVCode.Select(x => x.VCode));

        var vzerOnly = vertegenwoordigerPersoonsgegevensByVCode.Where(x => vzerOnlyVcodes.Contains(x.VCode))
                                                               .ToList();

        return vzerOnly;
    }
}
