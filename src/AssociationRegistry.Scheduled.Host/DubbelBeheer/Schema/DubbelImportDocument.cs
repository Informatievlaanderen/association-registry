namespace AssociationRegistry.Scheduled.Host.DubbelBeheer.Schema;

using Parsing;

/// <summary>
///     Eén rij (dubbele vereniging en haar authentieke vereniging) uit een ingelezen CSV-bestand.
///     <see cref="LineNumber" /> is de positie in het bestand, en dus ook de uitvoeringsvolgorde.
/// </summary>
public class DubbelImportDocument
{
    public Guid Id { get; set; }
    public Guid BatchId { get; set; }
    public string SourceKey { get; set; } = string.Empty;
    public int LineNumber { get; set; }
    public string DubbeleVCode { get; set; } = string.Empty;
    public string AuthentiekeVCode { get; set; } = string.Empty;
    public DubbelImportLineStatus Status { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;
    public DateTimeOffset IngestedAt { get; set; }

    public static IReadOnlyList<DubbelImportDocument> Create(
        Guid batchId,
        string sourceKey,
        IEnumerable<DubbelCsvRecord> records,
        DateTimeOffset ingestedAt
    ) =>
        records
            .Select(
                (x, index) =>
                    new DubbelImportDocument
                    {
                        Id = Guid.NewGuid(),
                        BatchId = batchId,
                        SourceKey = sourceKey,
                        LineNumber = index + 1,
                        DubbeleVCode = x.DubbeleVCode.Value,
                        AuthentiekeVCode = x.AuthentiekeVCode.Value,
                        Status = DubbelImportLineStatus.Open,
                        IngestedAt = ingestedAt,
                    }
            )
            .ToList();
}

public enum DubbelImportLineStatus
{
    Open,
    Processed,
    Failed,
}
