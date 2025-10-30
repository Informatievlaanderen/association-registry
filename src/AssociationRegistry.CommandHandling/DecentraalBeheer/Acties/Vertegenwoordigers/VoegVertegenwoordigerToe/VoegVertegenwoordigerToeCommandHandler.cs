namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.VoegVertegenwoordigerToe;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using JasperFx.Core;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Wolverine;
using Wolverine.Marten;

public class VoegVertegenwoordigerToeCommandHandler
{
    private readonly IVerenigingsRepository _repository;
    private readonly IMessageBus _outbox;

    public VoegVertegenwoordigerToeCommandHandler(IVerenigingsRepository verenigingRepository, IMessageBus outbox)
    {
        _repository = verenigingRepository;
        _outbox = outbox;
    }

    public async Task<EntityCommandResult> Handle(
        CommandEnvelope<VoegVertegenwoordigerToeCommand> envelope,
        CancellationToken cancellationToken = default)
    {
        var vereniging = await _repository.Load<Vereniging>(envelope.Command.VCode, envelope.Metadata)
                                          .OrWhenUnsupportedOperationForType()
                                          .Throw<VerenigingMetRechtspersoonlijkheidKanGeenVertegenwoordigersToevoegen>();

        var vertegenwoordigerId = vereniging.VoegVertegenwoordigerToe(envelope.Command.Vertegenwoordiger);

        await _outbox.SendAsync(new StartBewaartermijn(Guid.NewGuid().ToString()));

        var result = await _repository.Save(vereniging, envelope.Metadata, cancellationToken);


        return EntityCommandResult.Create(VCode.Create(envelope.Command.VCode), vertegenwoordigerId.VertegenwoordigerId, result);
    }
}

public record StartBewaartermijn(string Id);

public record BewaartermijnTimeout(string RefId) : TimeoutMessage(5.Minutes());

public class Bewaartermijn : Saga
{
    public string? Id { get; set; }

    // This method would be called when a StartBewaartermijn message arrives
    // to start a new Bewaartermijn
    public static (Bewaartermijn, BewaartermijnTimeout) Start(StartBewaartermijn bewaartermijn, ILogger<Bewaartermijn> logger)
    {
        logger.LogInformation("Got a new bewaartermijn with id {Id}", bewaartermijn.Id);

        // creating a timeout message for the saga
        return (new Bewaartermijn{Id = bewaartermijn.Id}, new BewaartermijnTimeout(bewaartermijn.Id));
    }

    // Delete this bewaartermijn if it has not already been deleted to enforce a "timeout"
    // condition
    public void Handle(BewaartermijnTimeout timeout, ILogger<Bewaartermijn> logger)
    {
        logger.LogInformation("Applying timeout to bewaartermijn {Id}", timeout.RefId);

        // That's it, we're done. Delete the saga state after the message is done.
        MarkCompleted();
    }
}

