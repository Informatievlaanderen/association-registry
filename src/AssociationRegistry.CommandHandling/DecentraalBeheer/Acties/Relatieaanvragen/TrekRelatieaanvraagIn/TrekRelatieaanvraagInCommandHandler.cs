namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.TrekRelatieaanvraagIn;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using Framework;

/// <summary>Only the relatieaanvraag is loaded: withdrawing changes nothing on the verenigingen.</summary>
public class TrekRelatieaanvraagInCommandHandler
{
    private readonly IRelatieaanvraagRepository _relatieaanvraagRepository;
    private readonly IClock _clock;

    public TrekRelatieaanvraagInCommandHandler(IRelatieaanvraagRepository relatieaanvraagRepository, IClock clock)
    {
        _relatieaanvraagRepository = relatieaanvraagRepository;
        _clock = clock;
    }

    public async Task<RelatieaanvraagCommandResult> Handle(
        CommandEnvelope<TrekRelatieaanvraagInCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        var command = envelope.Command;
        var today = _clock.Today;

        var relatieaanvraag = await _relatieaanvraagRepository.Load(
            command.AanvraagId,
            envelope.Metadata.ExpectedVersion
        );

        var ingetrokken = IngediendeRelatieaanvraag
            .Create(relatieaanvraag, command.IngetrokkenDoor, today)
            .TrekIn(today);

        var result = await _relatieaanvraagRepository.SaveAfsluiting(
            relatieaanvraag,
            ingetrokken,
            envelope.Metadata,
            cancellationToken
        );

        return RelatieaanvraagCommandResult.Create(relatieaanvraag.AanvraagId, result);
    }
}
