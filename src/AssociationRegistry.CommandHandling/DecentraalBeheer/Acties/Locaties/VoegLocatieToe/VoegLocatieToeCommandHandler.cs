namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Locaties.VoegLocatieToe;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar;
using Integrations.Grar.AdresMatch;
using Integrations.Grar.Clients;
using Marten;
using MartenDb.Store;
using ProbeerAdresTeMatchen;
using Wolverine.Marten;

public class VoegLocatieToeCommandHandler
{
    private readonly IAggregateSession _aggregateSession;
    private readonly IMartenOutbox _outbox;
    private readonly IDocumentSession _session;
    private readonly IGrarClient _grarClient;
    private readonly IGeotagsService _geotagsService;

    public VoegLocatieToeCommandHandler(
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

    public async Task<EntityCommandResult> Handle(
        CommandEnvelope<VoegLocatieToeCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _aggregateSession.Load<VerenigingOfAnyKind>(
            VCode.Create(envelope.Command.VCode),
            envelope.Metadata
        );

        var locatie = envelope.Command.Locatie;
        var toegevoegdeLocatie = vereniging.VoegLocatieToe(locatie);

        if (toegevoegdeLocatie.AdresId is not null)
            await vereniging.NeemAdresDetailOver(
                toegevoegdeLocatie.LocatieId,
                new GrarAddressVerrijkingsService(_grarClient),
                cancellationToken
            );
        else
            await _outbox.SendAsync(
                new ProbeerAdresTeMatchenCommand(
                    envelope.Command.VCode,
                    vereniging.UncommittedEvents.OfType<LocatieWerdToegevoegd>().Single().Locatie.LocatieId
                )
            );

        await vereniging.HerberekenGeotags(_geotagsService);

        var result = await _aggregateSession.Save(vereniging, _session, envelope.Metadata, cancellationToken);

        return EntityCommandResult.Create(VCode.Create(envelope.Command.VCode), toegevoegdeLocatie.LocatieId, result);
    }
}
