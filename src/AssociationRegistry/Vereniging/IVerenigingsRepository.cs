namespace AssociationRegistry.Vereniging;

using EventStore;
using Framework;

public interface IVerenigingsRepository
{
    Task<StreamActionResult> Save(Vereniging vereniging, CommandMetadata metadata, CancellationToken cancellationToken);
    Task<Vereniging> Load(VCode vCode, long? expectedVersion);
}
