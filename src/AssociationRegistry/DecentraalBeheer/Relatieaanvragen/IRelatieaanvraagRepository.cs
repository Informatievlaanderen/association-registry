namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen;

using Events;
using EventStore;
using Framework;
using Vereniging;

public interface IRelatieaanvraagRepository
{
    Task<OpenRelatieaanvragen> GetOpenRelatieaanvragen(
        VCode koepelVCode,
        VCode verenigingVCode,
        CancellationToken cancellationToken
    );

    /// <summary>
    ///     Saves a new relatieaanvraag. The database allows only one open relatieaanvraag per koepel and vereniging,
    ///     and one open subverenigingsaanvraag per vereniging: a concurrent second one fails.
    /// </summary>
    Task<StreamActionResult> SaveNew(
        Relatieaanvraag relatieaanvraag,
        CommandMetadata metadata,
        CancellationToken cancellationToken
    );

    /// <summary>Loads the relatieaanvraag; with expectedVersion (If-Match) it must still be at that version.</summary>
    Task<RelatieaanvraagState> Load(RelatieaanvraagId aanvraagId, long? expectedVersion);

    /// <summary>
    ///     Saves the confirmation and the relatie on the vereniging in one transaction: both, or nothing. The
    ///     relatieaanvraag no longer counts as open.
    ///     Returns the version of the relatieaanvraag (its ETag) and the highest sequence of both streams, so a client
    ///     can wait until the relatieaanvraag and the vereniging are both projected.
    /// </summary>
    Task<StreamActionResult> SaveBevestiging(
        RelatieaanvraagState relatieaanvraag,
        RelatieaanvraagWerdBevestigd bevestigd,
        VerenigingsBase vereniging,
        CommandMetadata metadata,
        CancellationToken cancellationToken
    );

    /// <summary>
    ///     Saves the refusal: the relatieaanvraag no longer counts as open. Returns its version (its ETag) and sequence.
    /// </summary>
    Task<StreamActionResult> SaveWeigering(
        RelatieaanvraagState relatieaanvraag,
        RelatieaanvraagWerdGeweigerd geweigerd,
        CommandMetadata metadata,
        CancellationToken cancellationToken
    );
}

public interface IRelatieaanvraagIdService
{
    Task<RelatieaanvraagId> GetNext();
}
