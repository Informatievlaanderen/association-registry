namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.WeigerRelatieaanvraag;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using Framework;

/// <summary>Only the relatieaanvraag is loaded: refusing changes nothing on the verenigingen.</summary>
public class WeigerRelatieaanvraagCommandHandler
{
    private readonly IRelatieaanvraagRepository _relatieaanvraagRepository;
    private readonly IClock _clock;

    public WeigerRelatieaanvraagCommandHandler(IRelatieaanvraagRepository relatieaanvraagRepository, IClock clock)
    {
        _relatieaanvraagRepository = relatieaanvraagRepository;
        _clock = clock;
    }

    public async Task<RelatieaanvraagCommandResult> Handle(
        CommandEnvelope<WeigerRelatieaanvraagCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        var command = envelope.Command;
        var today = _clock.Today;

        var relatieaanvraag = await _relatieaanvraagRepository.Load(
            command.AanvraagId,
            envelope.Metadata.ExpectedVersion
        );

        var geweigerd = OntvangenRelatieaanvraag
            .Create(relatieaanvraag, command.GeweigerdDoor, today)
            .Weiger(command.Reden, today);

        var result = await _relatieaanvraagRepository.SaveWeigering(
            relatieaanvraag,
            geweigerd,
            envelope.Metadata,
            cancellationToken
        );

        return RelatieaanvraagCommandResult.Create(relatieaanvraag.AanvraagId, result);
    }
}
