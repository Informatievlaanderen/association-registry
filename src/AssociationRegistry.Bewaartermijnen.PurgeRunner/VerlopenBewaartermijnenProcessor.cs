namespace AssociationRegistry.Bewaartermijnen.PurgeRunner;

using CommandHandling.Bewaartermijnen.Acties.Verlopen;
using Framework;
using Microsoft.Extensions.Logging;
using Queries;
using Wolverine;

public class VerlopenBewaartermijnenProcessor : IVerlopenBewaartermijnenProcessor
{
    private readonly IVerlopenBewaartermijnQuery _query;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<VerlopenBewaartermijnenProcessor> _logger;

    public VerlopenBewaartermijnenProcessor(
        IVerlopenBewaartermijnQuery query,
        IMessageBus messageBus,
        ILogger<VerlopenBewaartermijnenProcessor> logger
    )
    {
        _query = query;
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task SendVerlopenBewaartermijnen(CancellationToken cancellationToken)
    {
        var documents = await _query.ExecuteAsync(cancellationToken);

        _logger.LogInformation($"Er werden {documents.Count()} te verwijderen berichten gevonden.");

        foreach (var doc in documents)
        {
            var command = new CommandEnvelope<VerloopBewaartermijnCommand>(
                new VerloopBewaartermijnCommand(doc.VCode, doc.EntityId, doc.Reden, doc.Vervaldag),
                CommandMetadata.ForDigitaalVlaanderenProcess
            );

            await _messageBus.InvokeAsync(command, cancellationToken);
        }
    }
}

public interface IVerlopenBewaartermijnenProcessor
{
    Task SendVerlopenBewaartermijnen(CancellationToken cancellationToken);
}
