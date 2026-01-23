namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using EventStore;
using Framework;

public interface INewAggregateSession
{
    Task<StreamActionResult> SaveNew(
        Vereniging vereniging,
        CommandMetadata metadata,
        CancellationToken cancellationToken
    );

    Task<StreamActionResult> SaveNew(
        VerenigingMetRechtspersoonlijkheid vereniging,
        CommandMetadata metadata,
        CancellationToken cancellationToken
    );
}
