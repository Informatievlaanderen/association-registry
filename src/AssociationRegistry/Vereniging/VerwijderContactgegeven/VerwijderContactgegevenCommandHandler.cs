namespace AssociationRegistry.Vereniging.VerwijderContactgegeven;

using AssociationRegistry.Framework;
using AssociationRegistry.VCodes;

public class VerwijderContactgegevenCommandHandler
{
    private readonly IVerenigingsRepository _repository;

    public VerwijderContactgegevenCommandHandler(IVerenigingsRepository repository)
    {
        _repository = repository;
    }

    public async Task<CommandResult> Handle(CommandEnvelope<VerwijderContactgegevenCommand> message)
    {
        var vereniging = await _repository.Load(VCode.Create(message.Command.VCode), message.Metadata.ExpectedVersion);

        vereniging.VerwijderContactgegeven(message.Command.ContactgegevenId);

        var result = await _repository.Save(vereniging, message.Metadata);
        return CommandResult.Create(VCode.Create(message.Command.VCode), result);
    }

}
