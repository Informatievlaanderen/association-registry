namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.VoegVertegenwoordigerToe;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Messages;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using Framework;
using Integrations.Grar.Bewaartermijnen;
using Magda.Persoon;
using MartenDb.Store;
using Wolverine.Marten;

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
        IMartenOutbox martenOutbox,
        BewaartermijnOptions bewaartermijnOptions,
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


        if (vereniging.IsGestopt)
            await StartBewaartermijn(martenOutbox, bewaartermijnOptions, vereniging, vertegenwoordigerId);

        var result = await _aggregateSession.Save(vereniging, envelope.Metadata, cancellationToken);

        return EntityCommandResult.Create(
            VCode.Create(envelope.Command.VCode),
            vertegenwoordigerId.VertegenwoordigerId,
            result
        );
    }

    private static async Task StartBewaartermijn(
        IMartenOutbox martenOutbox,
        BewaartermijnOptions bewaartermijnOptions,
        Vereniging vereniging,
        Vertegenwoordiger vertegenwoordigerId)
    {
        var vervaldag = vereniging.Einddatum.ToInstant().PlusTicks(bewaartermijnOptions.Duration.Ticks);

        await martenOutbox.SendAsync(new StartBewaartermijnMessage(
                                         vereniging.VCode,
                                         PersoonsgegevensType.Vertegenwoordigers.Value,
                                         vertegenwoordigerId.VertegenwoordigerId,
                                         vervaldag,
                                         BewaartermijnReden.VerenigingWerdGestopt
                                     ));
    }
}
