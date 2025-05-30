namespace AssociationRegistry.DecentraalBeheer.Locaties.VoegLocatieToe;

using AssociationRegistry.Framework;
using AssociationRegistry.Messages;
using AssociationRegistry.Vereniging;
using Events;
using Grar.Clients;
using Marten;
using Wolverine.Marten;

public class VoegLocatieToeCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;
    private readonly IMartenOutbox _outbox;
    private readonly IDocumentSession _session;
    private readonly IGrarClient _grarClient;

    public VoegLocatieToeCommandHandler(
        IVerenigingsRepository verenigingRepository,
        IMartenOutbox outbox,
        IDocumentSession session,
        IGrarClient grarClient)
    {
        _verenigingRepository = verenigingRepository;
        _outbox = outbox;
        _session = session;
        _grarClient = grarClient;
    }

    public async Task<EntityCommandResult> Handle(CommandEnvelope<VoegLocatieToeCommand> envelope, CancellationToken cancellationToken = default)
    {
        var vereniging =
            await _verenigingRepository.Load<VerenigingOfAnyKind>(
                VCode.Create(envelope.Command.VCode),
                envelope.Metadata);

        var locatie = envelope.Command.Locatie;
        var toegevoegdeLocatie = vereniging.VoegLocatieToe(locatie);

        if (toegevoegdeLocatie.AdresId is not null)
        {
            await vereniging.NeemAdresDetailOver(toegevoegdeLocatie.LocatieId, _grarClient, cancellationToken);
        }
        else if(toegevoegdeLocatie.Adres is not null)
        {
            await _outbox.SendAsync(new TeAdresMatchenLocatieMessage(
                                        envelope.Command.VCode,
                                        vereniging.UncommittedEvents.OfType<LocatieWerdToegevoegd>()
                                                  .Single()
                                                  .Locatie
                                                  .LocatieId));
        }

        var result = await _verenigingRepository.Save(vereniging, _session, envelope.Metadata, cancellationToken);

        return EntityCommandResult.Create(VCode.Create(envelope.Command.VCode), toegevoegdeLocatie.LocatieId, result);
    }
}
