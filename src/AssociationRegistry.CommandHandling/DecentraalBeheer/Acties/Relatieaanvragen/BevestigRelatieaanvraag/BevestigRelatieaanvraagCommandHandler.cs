namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.BevestigRelatieaanvraag;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using Framework;
using MartenDb.Store;

public class BevestigRelatieaanvraagCommandHandler
{
    private readonly IAggregateSession _aggregateSession;
    private readonly IRelatieaanvraagRepository _relatieaanvraagRepository;
    private readonly IClock _clock;

    public BevestigRelatieaanvraagCommandHandler(
        IAggregateSession aggregateSession,
        IRelatieaanvraagRepository relatieaanvraagRepository,
        IClock clock
    )
    {
        _aggregateSession = aggregateSession;
        _relatieaanvraagRepository = relatieaanvraagRepository;
        _clock = clock;
    }

    public async Task<RelatieaanvraagCommandResult> Handle(
        CommandEnvelope<BevestigRelatieaanvraagCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        var command = envelope.Command;
        var today = _clock.Today;

        // If-Match is the version of the relatieaanvraag, not of the verenigingen.
        var relatieaanvraag = await _relatieaanvraagRepository.Load(
            command.AanvraagId,
            envelope.Metadata.ExpectedVersion
        );

        var ontvangen = OntvangenRelatieaanvraag.Create(relatieaanvraag, command.BevestigdDoor, today);

        var metadataWithoutVersion = envelope.Metadata with { ExpectedVersion = null };
        var koepel = await _aggregateSession.Load<VerenigingOfAnyKind>(
            relatieaanvraag.KoepelVCode,
            metadataWithoutVersion
        );
        var vereniging = await _aggregateSession.Load<VerenigingOfAnyKind>(
            relatieaanvraag.VerenigingVCode,
            metadataWithoutVersion
        );

        var openRelatieaanvragen = await _relatieaanvraagRepository.GetOpenRelatieaanvragen(
            relatieaanvraag.KoepelVCode,
            relatieaanvraag.VerenigingVCode,
            cancellationToken
        );

        var bevestigd = ontvangen.Bevestig(koepel, vereniging, openRelatieaanvragen, command.Relatiegegevens, today);

        var result = await _relatieaanvraagRepository.SaveBevestiging(
            relatieaanvraag,
            bevestigd,
            vereniging,
            envelope.Metadata,
            cancellationToken
        );

        return RelatieaanvraagCommandResult.Create(relatieaanvraag.AanvraagId, result);
    }
}
