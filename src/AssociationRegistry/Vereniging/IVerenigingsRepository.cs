namespace AssociationRegistry.Vereniging;

using EventStore;
using Framework;

public interface IVerenigingsRepository
{
    Task<StreamActionResult> Save(VerenigingsBase vereniging, CommandMetadata metadata, CancellationToken cancellationToken);
    Task<TVereniging> Load<TVereniging>(VCode vCode, long? expectedVersion) where TVereniging : IHydrate<VerenigingState>, new();
}
