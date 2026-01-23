namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Locaties.WijzigLocatie;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar;
using Integrations.Grar.AdresMatch;
using Integrations.Grar.Clients;
using Marten;
using MartenDb.Store;
using ProbeerAdresTeMatchen;
using Wolverine.Marten;

public class WijzigLocatieCommandHandler
{
    private readonly IAggregateSession _aggregateSession;
    private readonly IMartenOutbox _outbox;
    private readonly IDocumentSession _session;
    private readonly IGrarClient _grarClient;
    private readonly IGeotagsService _geotagsService;

    public WijzigLocatieCommandHandler(
        IAggregateSession aggregateSession,
        IMartenOutbox outbox,
        IDocumentSession session,
        IGrarClient grarClient,
        IGeotagsService geotagsService
    )
    {
        _aggregateSession = aggregateSession;
        _outbox = outbox;
        _session = session;
        _grarClient = grarClient;
        _geotagsService = geotagsService;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<WijzigLocatieCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _aggregateSession.Load<VerenigingOfAnyKind>(
            VCode.Create(envelope.Command.VCode),
            envelope.Metadata
        );

        var (locatieId, locatietype, isPrimair, naam, adres, adresId) = envelope.Command.TeWijzigenLocatie;
        vereniging.WijzigLocatie(locatieId, naam, locatietype, isPrimair, adresId, adres);

        await SynchroniseerLocatie(envelope, cancellationToken, adresId, vereniging, locatieId, adres);

        await vereniging.HerberekenGeotags(_geotagsService);

        var result = await _aggregateSession.Save(vereniging, _session, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }

    // TODO: refactor to class cfr: RegistreerFeitelijkeVerenigingCommandHandler, VoegLocatieToeCommandHandler
    private async Task SynchroniseerLocatie(
        CommandEnvelope<WijzigLocatieCommand> envelope,
        CancellationToken cancellationToken,
        AdresId? adresId,
        VerenigingOfAnyKind vereniging,
        int locatieId,
        Adres? adres
    )
    {
        if (adresId is not null)
        {
            await vereniging.NeemAdresDetailOver(
                locatieId,
                new GrarAddressVerrijkingsService(_grarClient),
                cancellationToken
            );
        }
        else if (adres is not null)
        {
            await _outbox.SendAsync(new ProbeerAdresTeMatchenCommand(envelope.Command.VCode, locatieId));
        }
    }
}
