namespace AssociationRegistry.Acties.SyncKbo;

using Framework;
using Vereniging;

public class SyncKboCommandHandler
{
    public async Task<CommandResult> Handle(
        CommandEnvelope<SyncKboCommand> message,
        IVerenigingsRepository repository,
        CancellationToken cancellationToken = default)
    {
        var vereniging = await repository.Load<VerenigingMetRechtspersoonlijkheid>(VCode.Create(message.Command.VCode), message.Metadata.ExpectedVersion);

        vereniging.WijzigNaamUitKbo(VerenigingsNaam.Create(message.Command.VerenigingVolgensKbo.Naam!));
        vereniging.WijzigKorteNaamUitKbo(message.Command.VerenigingVolgensKbo.KorteNaam);
        vereniging.WijzigContactgegevensUitKbo(message.Command.VerenigingVolgensKbo.Contactgegevens);

        vereniging.SyncCompleted();

        var result = await repository.Save(vereniging, message.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(message.Command.VCode), result);
    }
}
