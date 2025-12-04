namespace AssociationRegistry.Integrations.Magda.CallReferences;

using Shared.Models;

public interface IMagdaCallReferenceRepository
{
    Task Save(MagdaCallReference magdaCallReference, CancellationToken cancellationToken);
}
