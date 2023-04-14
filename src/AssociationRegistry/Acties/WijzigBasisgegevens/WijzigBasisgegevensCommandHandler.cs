﻿namespace AssociationRegistry.Acties.WijzigBasisgegevens;

using Framework;
using Vereniging;

public class WijzigBasisgegevensCommandHandler
{
    public async Task<CommandResult> Handle(CommandEnvelope<WijzigBasisgegevensCommand> message, IVerenigingsRepository repository)
    {
        var vereniging = await repository.Load(VCode.Create(message.Command.VCode), message.Metadata.ExpectedVersion);

        HandleNaam(vereniging, message.Command.Naam);
        HandleKorteNaam(vereniging, message.Command.KorteNaam);
        HandleKorteBeschrijving(vereniging, message.Command.KorteBeschrijving);
        HandleStartdatum(vereniging, message.Command.Startdatum);

        var result = await repository.Save(vereniging, message.Metadata);
        return CommandResult.Create(VCode.Create(message.Command.VCode), result);
    }

    private static void HandleStartdatum(Vereniging vereniging, Startdatum? startdatum)
    {
        if (startdatum is null)
            return;

        vereniging.WijzigStartdatum(startdatum);
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
}
