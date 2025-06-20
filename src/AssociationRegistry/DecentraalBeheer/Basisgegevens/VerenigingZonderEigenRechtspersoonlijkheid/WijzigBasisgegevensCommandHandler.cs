﻿namespace AssociationRegistry.DecentraalBeheer.Basisgegevens.FeitelijkeVereniging;

using AssociationRegistry.Framework;
using AssociationRegistry.Primitives;
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
        IClock clock,
        CancellationToken cancellationToken = default)
    {
        var vereniging = await repository.Load<Vereniging>(VCode.Create(message.Command.VCode), message.Metadata);

        HandleNaam(vereniging, message.Command.Naam);
        HandleKorteNaam(vereniging, message.Command.KorteNaam);
        HandleKorteBeschrijving(vereniging, message.Command.KorteBeschrijving);
        HandleStartdatum(vereniging, message.Command.Startdatum, clock);
        WijzigHoofdactiviteitenVerenigingsloket(vereniging, message.Command.HoofdactiviteitenVerenigingsloket);
        await WijzigWerkingsgebieden(vereniging, message.Command.Werkingsgebieden);
        HandleUitgeschrevenUitPubliekeDatastroom(vereniging, message.Command.IsUitgeschrevenUitPubliekeDatastroom);
        HandleDoelgroep(vereniging, message.Command.Doelgroep);

        var result = await repository.Save(vereniging, message.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(message.Command.VCode), result);
    }

    private static void HandleUitgeschrevenUitPubliekeDatastroom(
        Vereniging vereniging,
        bool? isUitgeschrevenUitPubliekeDatastroom)
    {
        if(isUitgeschrevenUitPubliekeDatastroom is not {} isUitgeschreven)
            return;

        if (isUitgeschreven)
            vereniging.SchrijfUitUitPubliekeDatastroom();
        else
            vereniging.SchrijfInInPubliekeDatastroom();
    }

    private static void WijzigHoofdactiviteitenVerenigingsloket(
        Vereniging vereniging,
        HoofdactiviteitVerenigingsloket[]? hoofdactiviteitenVerenigingsloket)
    {
        if (hoofdactiviteitenVerenigingsloket is null)
            return;

        vereniging.WijzigHoofdactiviteitenVerenigingsloket(hoofdactiviteitenVerenigingsloket);
    }

    private async Task WijzigWerkingsgebieden(
        Vereniging vereniging,
        Werkingsgebied[]? werkingsgebieden)
    {
        if (werkingsgebieden is null)
            return;

        if (vereniging.WijzigWerkingsgebieden(werkingsgebieden))
            await vereniging.BerekenGeotags(_geotagsService);
    }


    private static void HandleStartdatum(Vereniging vereniging, NullOrEmpty<Datum> startdatum, IClock clock)
    {
        if (startdatum.IsNull)
            return;

        vereniging.WijzigStartdatum(startdatum.IsEmpty ? null : startdatum.Value, clock);
    }

    private static void HandleKorteBeschrijving(Vereniging vereniging, string? korteBeschrijving)
    {
        if (korteBeschrijving is null) return;
        vereniging.WijzigKorteBeschrijving(korteBeschrijving);
    }

    private static void HandleNaam(Vereniging vereniging, VerenigingsNaam? naam)
    {
        if (naam is null) return;
        vereniging.WijzigNaam(naam);
    }

    private static void HandleKorteNaam(Vereniging vereniging, string? korteNaam)
    {
        if (korteNaam is null) return;
        vereniging.WijzigKorteNaam(korteNaam);
    }

    private static void HandleDoelgroep(Vereniging vereniging, Doelgroep? doelgroep)
    {
        if (doelgroep is null) return;
        vereniging.WijzigDoelgroep(doelgroep);
    }
}
