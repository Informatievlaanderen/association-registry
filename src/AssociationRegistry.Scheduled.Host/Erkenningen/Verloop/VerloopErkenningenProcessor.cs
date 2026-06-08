namespace AssociationRegistry.Scheduled.Host.Erkenningen.Verloop;

using CommandHandling.DecentraalBeheer.Acties.Erkenningen.ActiveerErkenning;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.VerloopErkenning;
using Framework;
using Microsoft.Extensions.Logging;
using Queries;
using Wolverine;

public class VerloopErkenningenProcessor : IVerloopErkenningenProcessor
{
    private readonly ITeVerlopenErkenningenQuery _query;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<VerloopErkenningenProcessor> _logger;

    public VerloopErkenningenProcessor(
        ITeVerlopenErkenningenQuery query,
        IMessageBus messageBus,
        ILogger<VerloopErkenningenProcessor> logger
    )
    {
        _query = query;
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task VerloopErkenningen(CancellationToken cancellationToken)
    {
        var erkenningen = await _query.ExecuteAsync(cancellationToken);

        _logger.LogInformation("Er werden {Aantal} te verlopen erkenningen gevonden.", erkenningen.Count);

        foreach (var erkenning in erkenningen)
        {
            var command = new CommandEnvelope<VerloopErkenningCommand>(
                new VerloopErkenningCommand(erkenning.VCode, erkenning.ErkenningId),
                CommandMetadata.ForDigitaalVlaanderenProcess
            );

            await _messageBus.SendAsync(command);
        }
    }
}

public interface IVerloopErkenningenProcessor
{
    Task VerloopErkenningen(CancellationToken cancellationToken);
}
