namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Lidmaatschappen.VoegLidmaatschapToe;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using Framework;
using MartenDb.Store;

public class VoegLidmaatschapToeCommandHandler
{
    private readonly IAggregateSession _aggregateSession;
    private readonly IVerenigingStateQueryService _queryService;

    public VoegLidmaatschapToeCommandHandler(
        IAggregateSession aggregateSession,
        IVerenigingStateQueryService queryService
    )
    {
        _aggregateSession = aggregateSession;
        _queryService = queryService;
    }

    public async Task<EntityCommandResult> Handle(
        CommandEnvelope<VoegLidmaatschapToeCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _aggregateSession.Load<VerenigingOfAnyKind>(
            VCode.Create(envelope.Command.VCode),
            metadata: envelope.Metadata
        );

        // todo: [technical debt] this is not safe.
        // QueryService should only be used if compensating actions exist on the 'other' vereniging.
        if (await _queryService.IsVerwijderd(envelope.Command.Lidmaatschap.AndereVereniging))
            throw new VerenigingKanGeenLidWordenVanVerwijderdeVereniging();

        var toegevoegdLidmaatschap = vereniging.VoegLidmaatschapToe(envelope.Command.Lidmaatschap);

        var result = await _aggregateSession.Save(
            vereniging: vereniging,
            metadata: envelope.Metadata,
            cancellationToken: cancellationToken
        );

        return EntityCommandResult.Create(
            VCode.Create(envelope.Command.VCode),
            entityId: toegevoegdLidmaatschap.LidmaatschapId,
            streamActionResult: result
        );
    }
}
