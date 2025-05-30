namespace AssociationRegistry.DecentraalBeheer.Locaties.WijzigLocatie;

using AssociationRegistry.Framework;
using AssociationRegistry.Messages;
using AssociationRegistry.Vereniging;
using Grar.Clients;
using Marten;
using Vereniging.Geotags;
using Wolverine.Marten;

public class WijzigLocatieCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;
    private readonly IMartenOutbox _outbox;
    private readonly IDocumentSession _session;
    private readonly IGrarClient _grarClient;
    private readonly IGeotagsService _geotagsService;

    public WijzigLocatieCommandHandler(
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

    public async Task<CommandResult> Handle(
        CommandEnvelope<WijzigLocatieCommand> envelope,
        CancellationToken cancellationToken = default)
    {
        var vereniging = await _verenigingRepository.Load<VerenigingOfAnyKind>(
            VCode.Create(envelope.Command.VCode),
            envelope.Metadata);


        var (locatieId, locatietype, isPrimair, naam, adres, adresId) = envelope.Command.TeWijzigenLocatie;
        vereniging.WijzigLocatie(locatieId, naam, locatietype, isPrimair, adresId, adres);

        await SynchroniseerLocatie(envelope, cancellationToken, adresId, vereniging, locatieId, adres);

        await vereniging.HerberekenGeotags(_geotagsService);

        var result = await _verenigingRepository.Save(vereniging, _session, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }

    // TODO: refactor to class cfr: RegistreerFeitelijkeVerenigingCommandHandler, VoegLocatieToeCommandHandler
    private async Task SynchroniseerLocatie(
        CommandEnvelope<WijzigLocatieCommand> envelope,
        CancellationToken cancellationToken,
        AdresId? adresId,
        VerenigingOfAnyKind vereniging,
        int locatieId,
        Adres? adres)
    {
        if (adresId is not null)
        {
            await vereniging.NeemAdresDetailOver(locatieId, _grarClient, cancellationToken);
        }
        else if(adres is not null)
        {
            await _outbox.SendAsync(new TeAdresMatchenLocatieMessage(envelope.Command.VCode, locatieId));
        }
    }
}
