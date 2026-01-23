namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.CorrigeerMarkeringAlsDubbelVan;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using Marten;
using MartenDb.Store;
using Reacties.AanvaardCorrectieDubbel;
using Wolverine.Marten;

public class CorrigeerMarkeringAlsDubbelVanCommandHandler
{
    private readonly IAggregateSession _aggregateSession;
    private readonly IMartenOutbox _outbox;
    private readonly IDocumentSession _session;

    public CorrigeerMarkeringAlsDubbelVanCommandHandler(
        IAggregateSession aggregateSession,
        IMartenOutbox outbox,
        IDocumentSession session
    )
    {
        _aggregateSession = aggregateSession;
        _outbox = outbox;
        _session = session;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<CorrigeerMarkeringAlsDubbelVanCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _aggregateSession.Load<Vereniging>(
            envelope.Command.VCode,
            envelope.Metadata,
            allowDubbeleVereniging: true
        );

        var vCodeAuthentiekeVereniging = vereniging.CorrigeerMarkeringAlsDubbelVan();

        await _outbox.SendAsync(
            new AanvaardCorrectieDubbeleVerenigingMessage(
                VCode.Create(vCodeAuthentiekeVereniging),
                envelope.Command.VCode
            )
        );

        var result = await _aggregateSession.Save(vereniging, _session, envelope.Metadata, cancellationToken);

        return CommandResult.Create(envelope.Command.VCode, result);
    }
}
