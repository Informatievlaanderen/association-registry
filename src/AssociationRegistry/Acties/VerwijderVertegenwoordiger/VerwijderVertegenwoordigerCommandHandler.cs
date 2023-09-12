namespace AssociationRegistry.Acties.VerwijderVertegenwoordiger;

using Framework;
using Vereniging;
using Vereniging.Exceptions;

public class VerwijderVertegenwoordigerCommandHandler
{
    private readonly IVerenigingsRepository _repository;

    public VerwijderVertegenwoordigerCommandHandler(IVerenigingsRepository repository)
    {
        _repository = repository;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<VerwijderVertegenwoordigerCommand> message,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var vereniging = await _repository.Load<Vereniging>(VCode.Create(message.Command.VCode), message.Metadata.ExpectedVersion);

            vereniging.VerwijderVertegenwoordiger(message.Command.VertegenwoordigerId);

            var result = await _repository.Save(vereniging, message.Metadata, cancellationToken);

            return CommandResult.Create(VCode.Create(message.Command.VCode), result);
        }
        catch (UnsupportedOperationForVerenigingstype)
        {
            throw new VerenigingMetRechtspersoonlijkheidCannotRemoveVertegenwoordigers();
        }
    }
}
