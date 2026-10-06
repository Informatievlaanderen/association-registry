namespace AssociationRegistry.CommandHandling.Geotags;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;
using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags.Messages;
using Framework;
using MartenDb.Store;

public class HerberekenGeotagsCommandHandler
{
    private readonly IAggregateSession _aggregateSession;
    private readonly IGeotagsService _geotagsService;

    public HerberekenGeotagsCommandHandler(IAggregateSession aggregateSession, IGeotagsService geotagsService)
    {
        _aggregateSession = aggregateSession;
        _geotagsService = geotagsService;
    }

    public async Task Handle(
        CommandEnvelope<HerberekenGeotagsCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _aggregateSession.Load<VerenigingOfAnyKind>(envelope.Command.VCode, envelope.Metadata);

        await vereniging.HerberekenGeotags(_geotagsService);

        await _aggregateSession.Save(vereniging, envelope.Metadata, cancellationToken);
    }
}
