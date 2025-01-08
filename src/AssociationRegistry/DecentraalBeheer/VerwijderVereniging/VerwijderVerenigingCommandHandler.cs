namespace AssociationRegistry.DecentraalBeheer.VerwijderVereniging;

using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;

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
        var vereniging = await _repository.Load<Vereniging>(message.Command.VCode, message.Metadata.ExpectedVersion)
                                          .OrWhenUnsupportedOperationForType()
                                          .Throw<VerenigingKanNietVerwijderdWorden>();

        vereniging.Verwijder(message.Command.Reden);

        var result = await _repository.Save(vereniging, message.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(message.Command.VCode), result);
    }
}
