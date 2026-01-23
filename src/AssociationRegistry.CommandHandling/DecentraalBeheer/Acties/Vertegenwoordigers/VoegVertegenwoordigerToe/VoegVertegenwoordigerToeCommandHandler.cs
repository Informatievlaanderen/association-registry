namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.VoegVertegenwoordigerToe;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using Magda.Persoon;
using MartenDb.Store;

public class VoegVertegenwoordigerToeCommandHandler
{
    private readonly IAggregateSession _aggregateSession;

    public VoegVertegenwoordigerToeCommandHandler(IAggregateSession aggregateSession)
    {
        _aggregateSession = aggregateSession;
    }

    public async Task<EntityCommandResult> Handle(
        CommandEnvelope<VoegVertegenwoordigerToeCommand> envelope,
        PersoonUitKsz persoonUitKsz,
        CancellationToken cancellationToken = default
    )
    {
        if (persoonUitKsz.Overleden)
            throw new OverledenVertegenwoordigerKanNietToegevoegdWorden();

        var vereniging = await _aggregateSession
            .Load<Vereniging>(envelope.Command.VCode, envelope.Metadata)
            .OrWhenUnsupportedOperationForType()
            .Throw<VerenigingMetRechtspersoonlijkheidKanGeenVertegenwoordigersToevoegen>();

        var vertegenwoordigerId = vereniging.VoegVertegenwoordigerToe(envelope.Command.Vertegenwoordiger);

        var result = await _aggregateSession.Save(vereniging, envelope.Metadata, cancellationToken);

        return EntityCommandResult.Create(
            VCode.Create(envelope.Command.VCode),
            vertegenwoordigerId.VertegenwoordigerId,
            result
        );
    }
}
