namespace AssociationRegistry.Scheduled.Host.DubbelBeheer;

using Schema;

/// <summary>
///     Maakt het rapport van een volledig verwerkte batch.
///     Schrijft naar een vaste key, zodat opnieuw genereren hetzelfde rapport overschrijft.
/// </summary>
public interface IDubbelImportReportGenerator
{
    Task Generate(DubbelImportBatchDocument batch, CancellationToken cancellationToken);
}
