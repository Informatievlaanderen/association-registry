namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.CorrigeerMarkeringAlsDubbelVan;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using Marten;
using Reacties.AanvaardCorrectieDubbel;
using Wolverine.Marten;

public class CorrigeerMarkeringAlsDubbelVanCommandHandler
{
    private readonly IVerenigingsRepository _verenigingsRepository;
    private readonly IMartenOutbox _outbox;
    private readonly IDocumentSession _session;

    public CorrigeerMarkeringAlsDubbelVanCommandHandler(
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
        CommandEnvelope<CorrigeerMarkeringAlsDubbelVanCommand> envelope,
        CancellationToken cancellationToken = default)
    {
        var vereniging = await _verenigingsRepository.Load<Vereniging>(envelope.Command.VCode, envelope.Metadata, allowDubbeleVereniging: true);

        var vCodeAuthentiekeVereniging = vereniging.CorrigeerMarkeringAlsDubbelVan();

        await _outbox.SendAsync(new AanvaardCorrectieDubbeleVerenigingMessage(VCode.Create(vCodeAuthentiekeVereniging), envelope.Command.VCode));

        var result = await _verenigingsRepository.Save(vereniging, _session, envelope.Metadata, cancellationToken);

        return CommandResult.Create(envelope.Command.VCode, result);
    }
}
