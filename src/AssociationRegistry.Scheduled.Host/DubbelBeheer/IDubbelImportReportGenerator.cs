namespace AssociationRegistry.Scheduled.Host.DubbelBeheer;

using Microsoft.Extensions.Logging;
using Schema;

/// <summary>
///     Maakt het rapport van een volledig verwerkte batch.
///     Schrijft naar een vaste key, zodat opnieuw genereren hetzelfde rapport overschrijft.
/// </summary>
public interface IDubbelImportReportGenerator
{
    Task Generate(DubbelImportBatchDocument batch, CancellationToken cancellationToken);
}

/// <summary>Voorlopige generator: het rapport in de report bucket volgt later.</summary>
public class LoggingDubbelImportReportGenerator(ILogger<LoggingDubbelImportReportGenerator> logger)
    : IDubbelImportReportGenerator
{
    public Task Generate(DubbelImportBatchDocument batch, CancellationToken cancellationToken)
    {
        logger.LogInformation("Rapport voor {SourceKey} wordt nog niet gegenereerd", batch.S3Key);

        return Task.CompletedTask;
    }
}
