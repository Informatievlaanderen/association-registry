namespace AssociationRegistry.Scheduled.Host.Erkenningen;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.ActiveerErkenning;
using AssociationRegistry.Framework;
using Microsoft.Extensions.Logging;
using Wolverine;

public class ErkenningenActivatieProcessor : IErkenningenActivatieProcessor
{
    private readonly ITeActiverenErkenningenQuery _query;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<ErkenningenActivatieProcessor> _logger;

    public ErkenningenActivatieProcessor(
        ITeActiverenErkenningenQuery query,
        IMessageBus messageBus,
        ILogger<ErkenningenActivatieProcessor> logger
    )
    {
        _query = query;
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task ActiveerErkenningen(CancellationToken cancellationToken)
    {
        var erkenningen = await _query.ExecuteAsync(cancellationToken);

        _logger.LogInformation("Er werden {Aantal} te activeren erkenningen gevonden.", erkenningen.Count);

        foreach (var erkenning in erkenningen)
        {
            var command = new CommandEnvelope<ActiveerErkenningCommand>(
                new ActiveerErkenningCommand(erkenning.VCode, erkenning.ErkenningId),
                CommandMetadata.ForDigitaalVlaanderenProcess
            );

            await _messageBus.InvokeAsync(command, cancellationToken);
        }
    }
}

public interface IErkenningenActivatieProcessor
{
    Task ActiveerErkenningen(CancellationToken cancellationToken);
}
