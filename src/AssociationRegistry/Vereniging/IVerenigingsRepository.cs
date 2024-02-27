namespace AssociationRegistry.Vereniging;

using EventStore;
using Framework;

public interface IVerenigingsRepository
{
    Task<StreamActionResult> Save(VerenigingsBase vereniging, CommandMetadata metadata, CancellationToken cancellationToken);
    Task<TVereniging> Load<TVereniging>(VCode vCode, long? expectedVersion = null) where TVereniging : IHydrate<VerenigingState>, new();
    Task<VerenigingMetRechtspersoonlijkheid?> Load(KboNummer kboNummer, long? expectedVersion = null);
}
