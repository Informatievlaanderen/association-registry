namespace AssociationRegistry.Acties.VoegLocatieToe;

using Events;
using Framework;
using Grar;
using Grar.AddressMatch;
using Grar.Exceptions;
using Marten;
using Vereniging;
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

    public async Task<CommandResult> Handle(CommandEnvelope<VoegLocatieToeCommand> envelope, CancellationToken cancellationToken = default)
    {
        var vereniging =
            await _verenigingRepository.Load<VerenigingOfAnyKind>(VCode.Create(envelope.Command.VCode), envelope.Metadata.ExpectedVersion);

        var locatie = envelope.Command.Locatie;
        vereniging.VoegLocatieToe(locatie);

        await SynchroniseerLocatie(envelope, cancellationToken, locatie, vereniging);

        var result = await _verenigingRepository.Save(vereniging, _session, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }

    private async Task SynchroniseerLocatie(
        CommandEnvelope<VoegLocatieToeCommand> envelope,
        CancellationToken cancellationToken,
        Locatie locatie,
        VerenigingOfAnyKind vereniging)
    {
        if (locatie.AdresId is not null)
        {
            await vereniging.NeemAdresDetailOver(locatie.LocatieId, locatie.AdresId, _grarClient, cancellationToken);
        }
        else if(locatie.Adres is not null)
        {
            await _outbox.SendAsync(new TeAdresMatchenLocatieMessage(
                                        envelope.Command.VCode,
                                        vereniging.UncommittedEvents.OfType<LocatieWerdToegevoegd>()
                                                  .Single()
                                                  .Locatie
                                                  .LocatieId));
        }
    }
}
