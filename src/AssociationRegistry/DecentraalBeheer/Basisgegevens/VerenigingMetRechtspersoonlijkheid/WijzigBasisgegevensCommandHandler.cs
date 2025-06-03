namespace AssociationRegistry.DecentraalBeheer.Basisgegevens.VerenigingMetRechtspersoonlijkheid;

using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using Vereniging.Geotags;

public class WijzigBasisgegevensCommandHandler
{
    private IGeotagsService _geotagsService;
    public WijzigBasisgegevensCommandHandler(IGeotagsService geotagsService)
    {
        _geotagsService = geotagsService;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<WijzigBasisgegevensCommand> message,
        IVerenigingsRepository repository,
        CancellationToken cancellationToken = default)
    {
        var vereniging =
            await repository.Load<VerenigingMetRechtspersoonlijkheid>(VCode.Create(message.Command.VCode),
                                                                      message.Metadata);

        HandleRoepnaam(vereniging, message.Command.Roepnaam);
        HandleKorteBeschrijving(vereniging, message.Command.KorteBeschrijving);
        HandleDoelgroep(vereniging, message.Command.Doelgroep);
        HandleHoofdactiviteitenVerenigingsloket(vereniging, message.Command.HoofdactiviteitenVerenigingsloket);
        HandleWerkingsgebieden(vereniging, message.Command.Werkingsgebieden);
        await vereniging.HerberekenGeotags(_geotagsService);

        var result = await repository.Save(vereniging, message.Metadata, cancellationToken);

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
        HoofdactiviteitVerenigingsloket[]? hoofdactiviteitenVerenigingsloket)
    {
        if (hoofdactiviteitenVerenigingsloket is null)
            return;

        vereniging.WijzigHoofdactiviteitenVerenigingsloket(hoofdactiviteitenVerenigingsloket);
    }

    private static void HandleWerkingsgebieden(
        VerenigingMetRechtspersoonlijkheid vereniging,
        Werkingsgebied[]? werkingsgebieden)
    {
        if (werkingsgebieden is null)
            return;

        vereniging.WijzigWerkingsgebieden(werkingsgebieden);
    }

    private static void HandleKorteBeschrijving(VerenigingMetRechtspersoonlijkheid vereniging, string? korteBeschrijving)
    {
        if (korteBeschrijving is null) return;
        vereniging.WijzigKorteBeschrijving(korteBeschrijving);
    }

    private static void HandleDoelgroep(VerenigingMetRechtspersoonlijkheid vereniging, Doelgroep? doelgroep)
    {
        if (doelgroep is null) return;
        vereniging.WijzigDoelgroep(doelgroep);
    }
}
