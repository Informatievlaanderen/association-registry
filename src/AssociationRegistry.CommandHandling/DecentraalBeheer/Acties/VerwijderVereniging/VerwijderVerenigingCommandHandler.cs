namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.VerwijderVereniging;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using System.Threading;
using System.Threading.Tasks;

public class VerwijderVerenigingCommandHandler
{
    private readonly IVerenigingsRepository _repository;

    public VerwijderVerenigingCommandHandler(IVerenigingsRepository repository)
    {
        _repository = repository;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<VerwijderVerenigingCommand> message,
        CancellationToken cancellationToken = default)
    {
        var vereniging = await _repository.Load<Vereniging>(message.Command.VCode, message.Metadata)
                                          .OrWhenUnsupportedOperationForType()
                                          .Throw<VerenigingKanNietVerwijderdWorden>();

        vereniging.Verwijder(message.Command.Reden);

        var result = await _repository.Save(vereniging, message.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(message.Command.VCode), result);
    }
}
