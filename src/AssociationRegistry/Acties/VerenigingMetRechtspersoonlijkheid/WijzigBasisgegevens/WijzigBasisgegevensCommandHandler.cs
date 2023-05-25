namespace AssociationRegistry.Acties.VerenigingMetRechtspersoonlijkheid.WijzigBasisgegevens;

using Framework;
using Vereniging;

public class WijzigBasisgegevensCommandHandler
{
    public async Task<CommandResult> Handle(
        CommandEnvelope<WijzigBasisgegevensCommand> message,
        IVerenigingsRepository repository,
        CancellationToken cancellationToken = default)
    {
        var vereniging = await repository.Load<VerenigingMetRechtspersoonlijkheid>(VCode.Create(message.Command.VCode), message.Metadata.ExpectedVersion);

        HandleKorteBeschrijving(vereniging, message.Command.KorteBeschrijving);
        HandleHoofdactiviteitenVerenigingsloket(vereniging, message.Command.HoofdactiviteitenVerenigingsloket);

        var result = await repository.Save(vereniging, message.Metadata, cancellationToken);
        return CommandResult.Create(VCode.Create(message.Command.VCode), result);
    }

    private static void HandleHoofdactiviteitenVerenigingsloket(VerenigingMetRechtspersoonlijkheid vereniging, HoofdactiviteitVerenigingsloket[]? hoofdactiviteitenVerenigingsloket)
    {
        if (hoofdactiviteitenVerenigingsloket is null)
            return;

        vereniging.WijzigHoofdactiviteitenVerenigingsloket(hoofdactiviteitenVerenigingsloket);
    }

    private static void HandleKorteBeschrijving(VerenigingMetRechtspersoonlijkheid vereniging, string? korteBeschrijving)
    {
        if (korteBeschrijving is null) return;
        vereniging.WijzigKorteBeschrijving(korteBeschrijving);
    }
}
