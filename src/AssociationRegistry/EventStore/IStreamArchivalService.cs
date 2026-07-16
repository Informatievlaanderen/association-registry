namespace AssociationRegistry.EventStore;

using DecentraalBeheer.Vereniging;

/// <summary>
/// Provides maintenance operations for archiving event streams.
/// </summary>
public interface IStreamArchivalService
{
    /// <summary>
    /// Archives the event stream for a given vereniging. The stream will no longer be
    /// retrievable through default queries, but the vCode remains reserved and will not
    /// be reused for new registrations (vCodes are issued from a monotonically increasing sequence).
    /// </summary>
    Task ArchiveStream(VCode vCode);
}
