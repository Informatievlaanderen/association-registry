namespace AssociationRegistry.Acties.MarkeerAlsDubbelVan;

using Framework;
using Marten;
using Messages;
using Vereniging;
using Vereniging.Exceptions;
using Wolverine.Marten;

public class MarkeerAlsDubbelVanCommandHandler
{
    private readonly IVerenigingsRepository _verenigingsRepository;
    private readonly IMartenOutbox _outbox;
    private readonly IDocumentSession _session;

    public MarkeerAlsDubbelVanCommandHandler(
        IVerenigingsRepository verenigingsRepository,
        IMartenOutbox outbox,
        IDocumentSession session
    )
    {
        _verenigingsRepository = verenigingsRepository;
        _outbox = outbox;
        _session = session;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<MarkeerAlsDubbelVanCommand> message,
        CancellationToken cancellationToken = default)
    {
        var vereniging = await _verenigingsRepository.Load<Vereniging>(message.Command.VCode, message.Metadata.ExpectedVersion);

        if (await _verenigingsRepository.IsVerwijderd(message.Command.VCodeAuthentiekeVereniging))
            throw new VerenigingKanGeenDubbelWordenVanVerwijderdeVereniging();

        if (await _verenigingsRepository.IsDubbel(message.Command.VCodeAuthentiekeVereniging))
            throw new VerenigingKanGeenDubbelWordenVanDubbelVereniging();

        vereniging.MarkeerAlsDubbelVan(message.Command.VCodeAuthentiekeVereniging);

        await _outbox.SendAsync(new VoegDubbelToeMessage(message.Command.VCodeAuthentiekeVereniging, message.Command.VCode));

        var result = await _verenigingsRepository.Save(vereniging, _session, message.Metadata, cancellationToken);

        return CommandResult.Create(message.Command.VCode, result);
    }
}
