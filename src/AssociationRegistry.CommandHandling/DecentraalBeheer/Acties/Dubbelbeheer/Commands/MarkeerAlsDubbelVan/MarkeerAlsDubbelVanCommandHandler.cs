namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.MarkeerAlsDubbelVan;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using Framework;
using MartenDb.Store;

/// <summary>
///     Markeert de dubbele vereniging en laat de authentieke vereniging ze aanvaarden in één transactie,
///     zodat beide verenigingen altijd samen gewijzigd worden.
///     Werd één van beide intussen gewijzigd, dan wordt alles opnieuw geprobeerd.
/// </summary>
public class MarkeerAlsDubbelVanCommandHandler
{
    private readonly IAggregateSession _aggregateSession;
    private readonly IVerenigingStateQueryService _queryService;

    public MarkeerAlsDubbelVanCommandHandler(
        IAggregateSession aggregateSession,
        IVerenigingStateQueryService queryService
    )
    {
        _aggregateSession = aggregateSession;
        _queryService = queryService;
    }

    public Task<CommandResult> Handle(
        CommandEnvelope<MarkeerAlsDubbelVanCommand> message,
        CancellationToken cancellationToken = default
    ) =>
        DubbelbeheerRetryPolicy.OnUnexpectedAggregateVersion.ExecuteAsync(
            _ => MarkeerAlsDubbelVan(message, cancellationToken),
            cancellationToken
        );

    private async Task<CommandResult> MarkeerAlsDubbelVan(
        CommandEnvelope<MarkeerAlsDubbelVanCommand> message,
        CancellationToken cancellationToken
    )
    {
        var vereniging = await _aggregateSession.Load<Vereniging>(
            vCode: message.Command.VCode,
            metadata: message.Metadata
        );

        await ThrowIfInvalidAuthentiekeVereniging(message);

        // De verwachte versie hoort bij de dubbele vereniging, niet bij de authentieke.
        var authentiekeVereniging = await _aggregateSession.Load<VerenigingOfAnyKind>(
            vCode: message.Command.VCodeAuthentiekeVereniging,
            metadata: message.Metadata with
            {
                ExpectedVersion = null,
            }
        );

        vereniging.MarkeerAlsDubbelVan(message.Command.VCodeAuthentiekeVereniging);
        authentiekeVereniging.AanvaardDubbeleVereniging(message.Command.VCode);

        var results = await _aggregateSession.SaveTransactional(
            verenigingen: [vereniging, authentiekeVereniging],
            metadata: message.Metadata,
            cancellationToken: cancellationToken
        );

        return CommandResult.Create(vCode: message.Command.VCode, streamActionResult: results[0]);
    }

    private async Task ThrowIfInvalidAuthentiekeVereniging(CommandEnvelope<MarkeerAlsDubbelVanCommand> message)
    {
        if (!await _queryService.Exists(message.Command.VCodeAuthentiekeVereniging))
            throw new VerenigingKanGeenDubbelWordenVanEenNietBestaandeVereniging();

        if (await _queryService.IsVerwijderd(message.Command.VCodeAuthentiekeVereniging))
            throw new VerenigingKanGeenDubbelWordenVanVerwijderdeVereniging();

        if (await _queryService.IsDubbel(message.Command.VCodeAuthentiekeVereniging))
            throw new VerenigingKanGeenDubbelWordenVanEenVerenigingReedsGemarkeerdAlsDubbel();
    }
}
