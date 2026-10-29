namespace AssociationRegistry.Scheduled.Host.DubbelBeheer;

using Schema;

/// <summary>Verwerkt één ingelezen rij. Een rij kan opnieuw aangeboden worden als een run halverwege stopt.</summary>
public interface IDubbelImportLineProcessor
{
    Task<DubbelImportLineResult> Process(DubbelImportDocument line, CancellationToken cancellationToken);
}

public record DubbelImportLineResult(DubbelImportLineStatus Status, string Reason)
{
    public static readonly DubbelImportLineResult Processed = new(DubbelImportLineStatus.Processed, string.Empty);

    public static DubbelImportLineResult Failed(string reason) => new(DubbelImportLineStatus.Failed, reason);
}
