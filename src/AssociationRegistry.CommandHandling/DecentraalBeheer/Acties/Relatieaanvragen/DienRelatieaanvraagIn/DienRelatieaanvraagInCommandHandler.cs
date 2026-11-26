namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.DienRelatieaanvraagIn;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using Framework;
using MartenDb.Store;

public class DienRelatieaanvraagInCommandHandler
{
    private readonly IAggregateSession _aggregateSession;
    private readonly IRelatieaanvraagRepository _relatieaanvraagRepository;
    private readonly IRelatieaanvraagIdService _aanvraagIdService;
    private readonly RelatieaanvraagOptions _options;
    private readonly IClock _clock;

    public DienRelatieaanvraagInCommandHandler(
        IAggregateSession aggregateSession,
        IRelatieaanvraagRepository relatieaanvraagRepository,
        IRelatieaanvraagIdService aanvraagIdService,
        RelatieaanvraagOptions options,
        IClock clock
    )
    {
        _aggregateSession = aggregateSession;
        _relatieaanvraagRepository = relatieaanvraagRepository;
        _aanvraagIdService = aanvraagIdService;
        _options = options;
        _clock = clock;
    }

    public async Task<RelatieaanvraagCommandResult> Handle(
        CommandEnvelope<DienRelatieaanvraagInCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        var command = envelope.Command;
        var partijen = command.Partijen;

        // The relatieaanvraag changes nothing on the verenigingen: they are only loaded to check the rules.
        var koepel = await _aggregateSession.Load<VerenigingOfAnyKind>(partijen.KoepelVCode, envelope.Metadata);
        var vereniging = await _aggregateSession.Load<VerenigingOfAnyKind>(partijen.VerenigingVCode, envelope.Metadata);

        var openRelatieaanvragen = await _relatieaanvraagRepository.GetOpenRelatieaanvragen(
            partijen.KoepelVCode,
            partijen.VerenigingVCode,
            cancellationToken
        );

        var relatieaanvraag = await Relatieaanvraag.DienIn(
            partijen,
            koepel,
            vereniging,
            command.RelatieType,
            command.Beschrijving,
            openRelatieaanvragen,
            _aanvraagIdService,
            _options.VervaltermijnInDagen,
            _clock
        );

        var result = await _relatieaanvraagRepository.SaveNew(relatieaanvraag, envelope.Metadata, cancellationToken);

        return RelatieaanvraagCommandResult.Create(relatieaanvraag.AanvraagId, result);
    }
}
