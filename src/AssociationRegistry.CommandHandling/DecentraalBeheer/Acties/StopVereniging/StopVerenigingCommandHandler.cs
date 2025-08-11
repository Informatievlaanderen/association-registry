namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.StopVereniging;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using System.Threading;
using System.Threading.Tasks;

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
        var vereniging = await _repository.Load<Vereniging>(message.Command.VCode, message.Metadata)
                                          .OrWhenUnsupportedOperationForType()
                                          .Throw<VerenigingMetRechtspersoonlijkheidKanNietGestoptWorden>();

        vereniging.Stop(message.Command.Einddatum, _clock);

        var result = await _repository.Save(vereniging, message.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(message.Command.VCode), result);
    }
}
