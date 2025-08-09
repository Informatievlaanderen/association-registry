namespace AssociationRegistry.DecentraalBeheer.Acties.Locaties.VoegLocatieToe;

using Events;
using Framework;
using Grar.Clients;
using Marten;
using ProbeerAdresTeMatchen;
using Vereniging;
using Vereniging.Geotags;
using Wolverine.Marten;

public class VoegLocatieToeCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;
    private readonly IMartenOutbox _outbox;
    private readonly IDocumentSession _session;
    private readonly IGrarClient _grarClient;
    private readonly IGeotagsService _geotagsService;

    public VoegLocatieToeCommandHandler(
        IVerenigingsRepository verenigingRepository,
        IMartenOutbox outbox,
        IDocumentSession session,
        IGrarClient grarClient,
        IGeotagsService geotagsService)
    {
        _verenigingRepository = verenigingRepository;
        _outbox = outbox;
        _session = session;
        _grarClient = grarClient;
        _geotagsService = geotagsService;
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
            await vereniging.NeemAdresDetailOver(toegevoegdeLocatie.LocatieId, _grarClient, cancellationToken);
        else
            await _outbox.SendAsync(new ProbeerAdresTeMatchenCommand(
                                        envelope.Command.VCode,
                                        vereniging.UncommittedEvents.OfType<LocatieWerdToegevoegd>()
                                                  .Single()
                                                  .Locatie
                                                  .LocatieId));

        await vereniging.HerberekenGeotags(_geotagsService);

        var result = await _verenigingRepository.Save(vereniging, _session, envelope.Metadata, cancellationToken);

        return EntityCommandResult.Create(VCode.Create(envelope.Command.VCode), toegevoegdeLocatie.LocatieId, result);
    }
}
