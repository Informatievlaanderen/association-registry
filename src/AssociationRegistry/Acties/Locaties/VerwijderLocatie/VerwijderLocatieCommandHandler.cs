namespace AssociationRegistry.Acties.Locaties.VerwijderLocatie;

using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;

public class VerwijderLocatieCommandHandler
{
    private readonly IVerenigingsRepository _repository;

    public VerwijderLocatieCommandHandler(IVerenigingsRepository repository)
    {
        _repository = repository;
    }

    public async Task<CommandResult> Handle(CommandEnvelope<VerwijderLocatieCommand> message, CancellationToken cancellationToken = default)
    {
        var vereniging = await _repository.Load<VerenigingOfAnyKind>(VCode.Create(message.Command.VCode), message.Metadata.ExpectedVersion);

        vereniging.VerwijderLocatie(message.Command.LocatieId);

        var result = await _repository.Save(vereniging, message.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(message.Command.VCode), result);
    }
}
