namespace AssociationRegistry.Scheduled.Host.DubbelBeheer.Schema;

using Parsing;

/// <summary>
///     Eén nog te verwerken rij (dubbele vereniging en haar authentieke vereniging) uit een ingelezen CSV-bestand.
///     <see cref="Volgnummer" /> is de positie in het bestand, en dus ook de uitvoeringsvolgorde.
/// </summary>
public class DubbelImportDocument
{
    public Guid Id { get; set; }
    public string SourceKey { get; set; } = string.Empty;
    public int Volgnummer { get; set; }
    public string DubbeleVCode { get; set; } = string.Empty;
    public string AuthentiekeVCode { get; set; } = string.Empty;
    public DateTimeOffset IngestedAt { get; set; }

    public static IReadOnlyList<DubbelImportDocument> Nieuw(
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
                        SourceKey = sourceKey,
                        Volgnummer = index + 1,
                        DubbeleVCode = x.DubbeleVCode.Value,
                        AuthentiekeVCode = x.AuthentiekeVCode.Value,
                        IngestedAt = ingestedAt,
                    }
            )
            .ToList();
}
