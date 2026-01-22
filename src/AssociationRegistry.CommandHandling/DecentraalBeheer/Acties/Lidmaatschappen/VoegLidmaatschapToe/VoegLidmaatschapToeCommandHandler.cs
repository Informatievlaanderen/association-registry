namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Lidmaatschappen.VoegLidmaatschapToe;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using Framework;
using MartenDb.Store;

public class VoegLidmaatschapToeCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;
    private readonly IVerenigingStateQueryService _queryService;

    public VoegLidmaatschapToeCommandHandler(
        IVerenigingsRepository verenigingRepository,
        IVerenigingStateQueryService queryService
    )
    {
        _verenigingRepository = verenigingRepository;
        _queryService = queryService;
    }

    public async Task<EntityCommandResult> Handle(
        CommandEnvelope<VoegLidmaatschapToeCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _verenigingRepository.Load<VerenigingOfAnyKind>(
            VCode.Create(envelope.Command.VCode),
            commandMetadata: envelope.Metadata
        );

        if (await _queryService.IsVerwijderd(envelope.Command.Lidmaatschap.AndereVereniging))
            throw new VerenigingKanGeenLidWordenVanVerwijderdeVereniging();

        var toegevoegdLidmaatschap = vereniging.VoegLidmaatschapToe(envelope.Command.Lidmaatschap);

        var result = await _verenigingRepository.Save(
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
