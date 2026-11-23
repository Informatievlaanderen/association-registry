namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen;

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
}

public interface IRelatieaanvraagIdService
{
    Task<RelatieaanvraagId> GetNext();
}
