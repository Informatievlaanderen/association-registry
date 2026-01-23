namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using EventStore;
using Framework;
using Marten;

/// <summary>
/// Provides efficient query operations for vereniging state without loading the full aggregate.
/// </summary>
public interface IVerenigingStateQueryService
{
    /// <summary>
    /// Checks if a vereniging has been deleted (verwijderd).
    /// </summary>
    Task<bool> IsVerwijderd(VCode vCode);

    /// <summary>
    /// Checks if a vereniging has been marked as duplicate (dubbel).
    /// </summary>
    Task<bool> IsDubbel(VCode vCode);

    /// <summary>
    /// Checks if a vereniging exists in the event stream.
    /// </summary>
    Task<bool> Exists(VCode vCode);

    /// <summary>
    /// Checks if a vereniging with the given KBO number exists.
    /// </summary>
    Task<bool> Exists(KboNummer kboNummer);
}

public interface INewAggregateSession
{
    Task<StreamActionResult> SaveNew(
        VerenigingsBase vereniging,
        IDocumentSession session,
        CommandMetadata metadata,
        CancellationToken cancellationToken
    );
}
