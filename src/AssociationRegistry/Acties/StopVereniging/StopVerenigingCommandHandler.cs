namespace AssociationRegistry.Acties.StopVereniging;

using Framework;
using ResultNet;
using Vereniging;

public class StopVerenigingCommandHandler
{
    private readonly IVerenigingsRepository _repository;
    private readonly IClock _clock;

    public StopVerenigingCommandHandler(IVerenigingsRepository repository, IClock clock)
    {
        _repository = repository;
        _clock = clock;
    }

    public async Task<CommandResult> Handle(CommandEnvelope<StopVerenigingCommand> message, CancellationToken cancellationToken = default)
    {
        var vereniging = await _repository.Load<Vereniging>(VCode.Create(message.Command.VCode), message.Metadata.ExpectedVersion);

        vereniging.Stop(message.Command.Einddatum, _clock);

        var result = await _repository.Save(vereniging, message.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(message.Command.VCode), result);
    }
}
