namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.VerwijderVertegenwoordiger;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;

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
        var vereniging = await _repository.Load<Vereniging>(message.Command.VCode, message.Metadata)
                                          .OrWhenUnsupportedOperationForType()
                                          .Throw<VerenigingMetRechtspersoonlijkheidKanGeenVertegenwoordigersVerwijderen>();

        vereniging.VerwijderVertegenwoordiger(message.Command.VertegenwoordigerId);

        var result = await _repository.Save(vereniging, message.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(message.Command.VCode), result);
    }
}
