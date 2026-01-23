namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Geotags.InitialiseerGeotags;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;
using AssociationRegistry.Framework;
using MartenDb.Store;

public class InitialiseerGeotagsCommandHandler
{
    private readonly IAggregateSession _aggregateSession;
    private readonly IGeotagsService _geotagsService;

    public InitialiseerGeotagsCommandHandler(IAggregateSession aggregateSession, IGeotagsService geotagsService)
    {
        _aggregateSession = aggregateSession;
        _geotagsService = geotagsService;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<InitialiseerGeotagsCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _aggregateSession.Load<VerenigingOfAnyKind>(
            VCode.Create(envelope.Command.VCode),
            envelope.Metadata,
            allowVerwijderdeVereniging: true,
            allowDubbeleVereniging: true
        );

        await vereniging.InitialiseerGeotags(_geotagsService);

        var result = await _aggregateSession.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
