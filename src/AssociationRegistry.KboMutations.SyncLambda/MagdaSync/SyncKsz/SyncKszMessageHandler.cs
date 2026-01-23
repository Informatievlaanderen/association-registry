namespace AssociationRegistry.KboMutations.SyncLambda.MagdaSync.SyncKsz;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.Persoonsgegevens;
using MartenDb.Store;
using Microsoft.Extensions.Logging;
using NodaTime;
using Queries;
using Wolverine;

public class SyncKszMessageHandler
{
    private readonly IVertegenwoordigerPersoonsgegevensRepository _vertegenwoordigerPersoonsgegevensRepository;
    private readonly IAggregateSession _aggregateSession;
    private readonly IFilterVzerOnlyQuery _filterVzerOnlyQuery;
    private readonly ILogger<SyncKszMessageHandler> _logger;

    public SyncKszMessageHandler(
        IVertegenwoordigerPersoonsgegevensRepository vertegenwoordigerPersoonsgegevensRepository,
        IAggregateSession aggregateSession,
        IFilterVzerOnlyQuery filterVzerOnlyQuery,
        ILogger<SyncKszMessageHandler> logger
    )
    {
        _vertegenwoordigerPersoonsgegevensRepository = vertegenwoordigerPersoonsgegevensRepository;
        _aggregateSession = aggregateSession;
        _filterVzerOnlyQuery = filterVzerOnlyQuery;
        _logger = logger;
    }

    public async Task Handle(
        CommandEnvelope<SyncKszMessage> messageEnvelope,
        CancellationToken cancellationToken = default
    )
    {
        var message = messageEnvelope.Command;
        if (!message.Overleden) // only need to sync if overleden for now
        {
            _logger.LogInformation("Skipping message because this person is not deceased");

            return;
        }

        var vertegenwoordigerPersoonsgegevens = await _vertegenwoordigerPersoonsgegevensRepository.Get(
            message.Insz,
            cancellationToken
        );

        if (!vertegenwoordigerPersoonsgegevens.Any())
        {
            // TODO: uitschrijven or-2939

            _logger.LogWarning(
                "Skipping message because this person did not match any known VertegenwoordigerPersoonsgegevensDocument"
            );

            return;
        }

        var vzerOnly = await FilterOnlyVzer(vertegenwoordigerPersoonsgegevens, cancellationToken);

        if (!vzerOnly.Any())
        {
            _logger.LogInformation("Only found kbo associations for this person");
        }

        foreach (var vertegenwoordigerPersoonsgegeven in vzerOnly)
        {
            _logger.LogInformation($"trying to load vcode: {vertegenwoordigerPersoonsgegeven.VCode}");

            var vereniging = await _aggregateSession.Load<Vereniging>(
                VCode.Create(vertegenwoordigerPersoonsgegeven.VCode),
                messageEnvelope.Metadata,
                allowDubbeleVereniging: true,
                allowVerwijderdeVereniging: true
            );

            vereniging.MarkeerVertegenwoordigerAlsOverleden(vertegenwoordigerPersoonsgegeven.VertegenwoordigerId);

            await _aggregateSession.Save(vereniging, messageEnvelope.Metadata, cancellationToken);

            _logger.LogInformation(
                $"SyncKszMessageHandler marked vertegenwoordiger as deceased with vCode: {vertegenwoordigerPersoonsgegeven.VCode} for vertegenwoordiger: {vertegenwoordigerPersoonsgegeven.VertegenwoordigerId}"
            );
        }

        _logger.LogInformation("SyncKszMessageHandler done");
    }

    private async Task<List<VertegenwoordigerPersoonsgegevens>> FilterOnlyVzer(
        VertegenwoordigerPersoonsgegevens[] vertegenwoordigerPersoonsgegevens,
        CancellationToken cancellationToken
    )
    {
        var vertegenwoordigerPersoonsgegevensByVCode = vertegenwoordigerPersoonsgegevens
            .DistinctBy(x => x.VCode)
            .ToList();
        var vzerOnlyVcodes = await _filterVzerOnlyQuery.ExecuteAsync(
            new FilterVzerOnlyQueryFilter(vertegenwoordigerPersoonsgegevensByVCode.Select(x => x.VCode).ToArray()),
            cancellationToken
        );

        var vzerOnly = vertegenwoordigerPersoonsgegevensByVCode.Where(x => vzerOnlyVcodes.Contains(x.VCode)).ToList();

        return vzerOnly;
    }
}
