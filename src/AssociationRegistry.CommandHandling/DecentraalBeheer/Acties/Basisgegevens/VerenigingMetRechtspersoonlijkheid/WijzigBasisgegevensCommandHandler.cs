namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Basisgegevens.VerenigingMetRechtspersoonlijkheid;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;
using AssociationRegistry.Framework;
using MartenDb.Store;

public class WijzigBasisgegevensCommandHandler
{
    private readonly IGeotagsService _geotagsService;

    public WijzigBasisgegevensCommandHandler(IGeotagsService geotagsService)
    {
        _geotagsService = geotagsService;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<WijzigBasisgegevensCommand> message,
        IAggregateSession aggregateSession,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await aggregateSession.Load<VerenigingMetRechtspersoonlijkheid>(
            VCode.Create(message.Command.VCode),
            message.Metadata
        );

        HandleRoepnaam(vereniging, message.Command.Roepnaam);
        HandleKorteBeschrijving(vereniging, message.Command.KorteBeschrijving);
        HandleDoelgroep(vereniging, message.Command.Doelgroep);
        HandleHoofdactiviteitenVerenigingsloket(vereniging, message.Command.HoofdactiviteitenVerenigingsloket);
        await HandleWerkingsgebieden(vereniging, message.Command.Werkingsgebieden);

        var result = await aggregateSession.Save(vereniging, message.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(message.Command.VCode), result);
    }

    private void HandleRoepnaam(VerenigingMetRechtspersoonlijkheid vereniging, string? roepnaam)
    {
        if (roepnaam is null)
            return;

        vereniging.WijzigRoepnaam(roepnaam);
    }

    private static void HandleHoofdactiviteitenVerenigingsloket(
        VerenigingMetRechtspersoonlijkheid vereniging,
        HoofdactiviteitVerenigingsloket[]? hoofdactiviteitenVerenigingsloket
    )
    {
        if (hoofdactiviteitenVerenigingsloket is null)
            return;

        vereniging.WijzigHoofdactiviteitenVerenigingsloket(hoofdactiviteitenVerenigingsloket);
    }

    private async Task HandleWerkingsgebieden(
        VerenigingMetRechtspersoonlijkheid vereniging,
        Werkingsgebied[]? werkingsgebieden
    )
    {
        if (werkingsgebieden is null)
            return;

        if (vereniging.WijzigWerkingsgebieden(werkingsgebieden))
            await vereniging.HerberekenGeotags(_geotagsService);
    }

    private static void HandleKorteBeschrijving(
        VerenigingMetRechtspersoonlijkheid vereniging,
        string? korteBeschrijving
    )
    {
        if (korteBeschrijving is null)
            return;
        vereniging.WijzigKorteBeschrijving(korteBeschrijving);
    }

    private static void HandleDoelgroep(VerenigingMetRechtspersoonlijkheid vereniging, Doelgroep? doelgroep)
    {
        if (doelgroep is null)
            return;
        vereniging.WijzigDoelgroep(doelgroep);
    }
}
