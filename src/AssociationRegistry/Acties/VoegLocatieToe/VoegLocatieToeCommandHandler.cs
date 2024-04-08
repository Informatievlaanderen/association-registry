namespace AssociationRegistry.Acties.VoegLocatieToe;

using Events;
using Framework;
using Grar.AddressMatch;
using Marten;
using Vereniging;
using Wolverine.Marten;

public class VoegLocatieToeCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;
    private readonly IMartenOutbox _outbox;
    private readonly IDocumentSession _session;

    public VoegLocatieToeCommandHandler(
        IVerenigingsRepository verenigingRepository,
        IMartenOutbox outbox,
        IDocumentSession session
        )
    {
        _verenigingRepository = verenigingRepository;
        _outbox = outbox;
        _session = session;
    }

    public async Task<CommandResult> Handle(CommandEnvelope<VoegLocatieToeCommand> envelope, CancellationToken cancellationToken = default)
    {
        var vereniging =
            await _verenigingRepository.Load<VerenigingOfAnyKind>(VCode.Create(envelope.Command.VCode), envelope.Metadata.ExpectedVersion);

        vereniging.VoegLocatieToe(envelope.Command.Locatie);

        await _outbox.SendAsync(new TeSynchroniserenAdresMessage(
                                    envelope.Command.VCode,
                                    vereniging.UncommittedEvents.OfType<LocatieWerdToegevoegd>()
                                              .Single()
                                              .Locatie
                                              .LocatieId));

        var result = await _verenigingRepository.Save(vereniging, _session, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
