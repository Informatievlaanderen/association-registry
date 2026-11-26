namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.VervalRelatieaanvraag;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using AssociationRegistry.Formats;
using EventStore;
using Framework;
using JasperFx.Core;
using Microsoft.Extensions.Logging;
using Wolverine.ErrorHandling;
using Wolverine.Runtime.Handlers;

/// <summary>
///     Runs in the scheduled host, from a durable Postgres queue, so a command can come more than once. The handler loads
///     the relatieaanvraag and checks again: a relatieaanvraag that is already closed, or not over yet, is skipped.
///     Only the relatieaanvraag is loaded: verval does not depend on the verenigingen, and changes nothing on them.
/// </summary>
public class VervalRelatieaanvraagCommandHandler
{
    private readonly IRelatieaanvraagRepository _relatieaanvraagRepository;
    private readonly IClock _clock;
    private readonly ILogger<VervalRelatieaanvraagCommandHandler> _logger;

    public VervalRelatieaanvraagCommandHandler(
        IRelatieaanvraagRepository relatieaanvraagRepository,
        IClock clock,
        ILogger<VervalRelatieaanvraagCommandHandler> logger
    )
    {
        _relatieaanvraagRepository = relatieaanvraagRepository;
        _clock = clock;
        _logger = logger;
    }

    /// <summary>
    ///     A version conflict: something else saved on the relatieaanvraag between loading and saving, like a second
    ///     command for it. The retry loads it again and skips it. After the retries the message goes to the error queue.
    /// </summary>
    public static void Configure(HandlerChain chain) =>
        chain
            .OnException<UnexpectedAggregateVersionException>()
            .RetryWithCooldown(1.Seconds(), 5.Seconds(), 15.Seconds())
            .Then.MoveToErrorQueue();

    public async Task Handle(
        CommandEnvelope<VervalRelatieaanvraagCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        var today = _clock.Today;

        var relatieaanvraag = await _relatieaanvraagRepository.Load(
            RelatieaanvraagId.Hydrate(envelope.Command.AanvraagId),
            expectedVersion: null
        );

        if (!relatieaanvraag.KanVervallenOp(today))
        {
            _logger.LogInformation(
                "Relatieaanvraag {AanvraagId} werd overgeslagen: status {Status}, vervaldatum {VervalDatum}.",
                relatieaanvraag.AanvraagId.Value,
                relatieaanvraag.Status.Value,
                relatieaanvraag.VervalDatum.FormatAsBelgianDate()
            );

            return;
        }

        await _relatieaanvraagRepository.SaveAfsluiting(
            relatieaanvraag,
            relatieaanvraag.Verval(today),
            envelope.Metadata,
            cancellationToken
        );
    }
}
