namespace AssociationRegistry.Scheduled.Host.Relatieaanvragen.Verval;

using CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.VervalRelatieaanvraag;
using Framework;
using Microsoft.Extensions.Logging;
using Queries;
using Wolverine;

/// <summary>
///     Sends a command per relatieaanvraag to the durable Postgres queue: once SendAsync returns, the command is saved in
///     Postgres and survives a restart. The handler checks every relatieaanvraag again on its own stream.
/// </summary>
public class VervalRelatieaanvragenProcessor : IVervalRelatieaanvragenProcessor
{
    private readonly ITeVervallenRelatieaanvragenQuery _query;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<VervalRelatieaanvragenProcessor> _logger;

    public VervalRelatieaanvragenProcessor(
        ITeVervallenRelatieaanvragenQuery query,
        IMessageBus messageBus,
        ILogger<VervalRelatieaanvragenProcessor> logger
    )
    {
        _query = query;
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task VervalRelatieaanvragen(CancellationToken cancellationToken)
    {
        var relatieaanvragen = await _query.ExecuteAsync(cancellationToken);

        _logger.LogInformation("Er werden {Aantal} te vervallen relatieaanvragen gevonden.", relatieaanvragen.Count);

        foreach (var relatieaanvraag in relatieaanvragen)
        {
            var command = new CommandEnvelope<VervalRelatieaanvraagCommand>(
                new VervalRelatieaanvraagCommand(relatieaanvraag.AanvraagId),
                CommandMetadata.ForDigitaalVlaanderenProcess
            );

            await _messageBus.SendAsync(command);
        }
    }
}

public interface IVervalRelatieaanvragenProcessor
{
    Task VervalRelatieaanvragen(CancellationToken cancellationToken);
}
