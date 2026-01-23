namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Locaties.WijzigMaatschappelijkeZetel;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;
using AssociationRegistry.Framework;
using MartenDb.Store;

public class WijzigMaatschappelijkeZetelCommandHandler
{
    private readonly IAggregateSession _aggregateSession;
    private readonly IGeotagsService _geotagsService;

    public WijzigMaatschappelijkeZetelCommandHandler(IAggregateSession aggregateSession, IGeotagsService geotagsService)
    {
        _aggregateSession = aggregateSession;
        _geotagsService = geotagsService;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<WijzigMaatschappelijkeZetelCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _aggregateSession.Load<VerenigingMetRechtspersoonlijkheid>(
            VCode.Create(envelope.Command.VCode),
            envelope.Metadata
        );

        vereniging.WijzigMaatschappelijkeZetel(
            envelope.Command.TeWijzigenLocatie.LocatieId,
            envelope.Command.TeWijzigenLocatie.Naam,
            envelope.Command.TeWijzigenLocatie.IsPrimair
        );

        await vereniging.HerberekenGeotags(_geotagsService);

        var result = await _aggregateSession.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
