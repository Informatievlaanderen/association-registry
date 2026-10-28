namespace AssociationRegistry.Scheduled.Host.DubbelBeheer;

using Microsoft.Extensions.Logging;
using Schema;

/// <summary>Verwerkt één ingelezen rij. Een rij kan opnieuw aangeboden worden als een run halverwege stopt.</summary>
public interface IDubbelImportLineProcessor
{
    Task Process(DubbelImportDocument line, CancellationToken cancellationToken);
}

/// <summary>Voorlopige verwerker: logt de rij enkel, het effectief markeren als dubbel volgt later.</summary>
public class LoggingDubbelImportLineProcessor(ILogger<LoggingDubbelImportLineProcessor> logger)
    : IDubbelImportLineProcessor
{
    public Task Process(DubbelImportDocument line, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Rij {LineNumber} uit {SourceKey}: {DubbeleVCode} is dubbel van {AuthentiekeVCode} (nog niet verwerkt)",
            line.LineNumber,
            line.SourceKey,
            line.DubbeleVCode,
            line.AuthentiekeVCode
        );

        return Task.CompletedTask;
    }
}
