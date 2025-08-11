namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Contactgegevens.VerwijderContactgegeven;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;

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
        var vereniging = await _repository.Load<VerenigingOfAnyKind>(VCode.Create(message.Command.VCode), message.Metadata);

        vereniging.VerwijderContactgegeven(message.Command.ContactgegevenId);

        var result = await _repository.Save(vereniging, message.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(message.Command.VCode), result);
    }
}
