namespace AssociationRegistry.DecentraalBeheer.Contactgegevens.VerwijderContactgegeven;

using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;

public class VerwijderContactgegevenCommandHandler
{
    private readonly IVerenigingsRepository _repository;

    public VerwijderContactgegevenCommandHandler(IVerenigingsRepository repository)
    {
        _repository = repository;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<VerwijderContactgegevenCommand> message,
        CancellationToken cancellationToken = default)
    {
        var vereniging = await _repository.Load<VerenigingOfAnyKind>(VCode.Create(message.Command.VCode), message.Metadata.ExpectedVersion);

        vereniging.VerwijderContactgegeven(message.Command.ContactgegevenId);

        var result = await _repository.Save(vereniging, message.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(message.Command.VCode), result);
    }
}
