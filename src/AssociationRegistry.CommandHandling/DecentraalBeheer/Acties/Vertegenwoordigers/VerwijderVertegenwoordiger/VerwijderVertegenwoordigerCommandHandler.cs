namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.VerwijderVertegenwoordiger;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using Bewaartermijnen.Acties.Start;
using System.Threading;
using System.Threading.Tasks;
using Wolverine.Marten;

public class VerwijderVertegenwoordigerCommandHandler
{
    private readonly IVerenigingsRepository _repository;
    private readonly IMartenOutbox _outbox;

    public VerwijderVertegenwoordigerCommandHandler(IVerenigingsRepository repository, IMartenOutbox outbox)
    {
        _repository = repository;
        _outbox = outbox;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<VerwijderVertegenwoordigerCommand> message,
        CancellationToken cancellationToken = default)
    {
        var vereniging = await _repository.Load<Vereniging>(message.Command.VCode, message.Metadata)
                                          .OrWhenUnsupportedOperationForType()
                                          .Throw<VerenigingMetRechtspersoonlijkheidKanGeenVertegenwoordigersVerwijderen>();

        vereniging.VerwijderVertegenwoordiger(message.Command.VertegenwoordigerId);

        await _outbox.SendAsync(new StartBewaartermijnMessage(message.Command.VCode, message.Command.VertegenwoordigerId));

        var result = await _repository.Save(vereniging, message.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(message.Command.VCode), result);
    }
}
