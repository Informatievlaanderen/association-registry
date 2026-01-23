namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Locaties.VerwijderLocatie;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;
using AssociationRegistry.Framework;
using MartenDb.Store;

public class VerwijderLocatieCommandHandler
{
    private readonly IAggregateSession _aggregateSession;
    private readonly IGeotagsService _geotagsService;

    public VerwijderLocatieCommandHandler(IAggregateSession aggregateSession, IGeotagsService geotagsService)
    {
        _aggregateSession = aggregateSession;
        _geotagsService = geotagsService;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<VerwijderLocatieCommand> message,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _aggregateSession.Load<VerenigingOfAnyKind>(
            VCode.Create(message.Command.VCode),
            message.Metadata
        );

        vereniging.VerwijderLocatie(message.Command.LocatieId);

        await vereniging.HerberekenGeotags(_geotagsService);

        var result = await _aggregateSession.Save(vereniging, message.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(message.Command.VCode), result);
    }
}
