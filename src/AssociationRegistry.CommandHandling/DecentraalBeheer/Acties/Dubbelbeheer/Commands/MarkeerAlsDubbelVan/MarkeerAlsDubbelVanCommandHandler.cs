namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.MarkeerAlsDubbelVan;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using Framework;
using Marten;
using MartenDb.Store;
using Reacties.AanvaardDubbel;
using Wolverine.Marten;

public class MarkeerAlsDubbelVanCommandHandler
{
    private readonly IAggregateSession _aggregateSession;
    private readonly IVerenigingStateQueryService _queryService;
    private readonly IMartenOutbox _outbox;
    private readonly IDocumentSession _session;

    public MarkeerAlsDubbelVanCommandHandler(
        IAggregateSession aggregateSession,
        IVerenigingStateQueryService queryService,
        IMartenOutbox outbox,
        IDocumentSession session
    )
    {
        _aggregateSession = aggregateSession;
        _queryService = queryService;
        _outbox = outbox;
        _session = session;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<MarkeerAlsDubbelVanCommand> message,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _aggregateSession.Load<Vereniging>(
            vCode: message.Command.VCode,
            metadata: message.Metadata
        );

        await ThrowIfInvalidAuthentiekeVereniging(message);

        vereniging.MarkeerAlsDubbelVan(message.Command.VCodeAuthentiekeVereniging);

        await _outbox.SendAsync(
            new AanvaardDubbeleVerenigingMessage(
                VCode: message.Command.VCodeAuthentiekeVereniging,
                VCodeDubbeleVereniging: message.Command.VCode
            )
        );

        var result = await _aggregateSession.Save(
            vereniging: vereniging,
            session: _session,
            metadata: message.Metadata,
            cancellationToken: cancellationToken
        );

        return CommandResult.Create(vCode: message.Command.VCode, streamActionResult: result);
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
