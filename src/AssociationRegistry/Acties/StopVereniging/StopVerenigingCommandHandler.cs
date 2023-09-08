namespace AssociationRegistry.Acties.StopVereniging;

using Framework;
using Vereniging;
using Vereniging.Exceptions;

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
        try
        {
            var vereniging = await _repository.Load<Vereniging>(VCode.Create(message.Command.VCode), message.Metadata.ExpectedVersion);
            vereniging.Stop(message.Command.Einddatum, _clock);

            var result = await _repository.Save(vereniging, message.Metadata, cancellationToken);

            return CommandResult.Create(VCode.Create(message.Command.VCode), result);

        }
        catch (UnsupportedOperationForVerenigingstype)
        {
            throw new VerenigingMetRechtspersoonlijkheidCannotBeStopped();
        }
    }
}
