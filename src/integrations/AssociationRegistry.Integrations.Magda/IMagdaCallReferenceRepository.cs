namespace AssociationRegistry.Integrations.Magda;

using Marten;
using Models;

public interface IMagdaCallReferenceRepository
{
    Task Save(MagdaCallReference magdaCallReference, CancellationToken cancellationToken);
}

public class MagdaCallReferenceRepository : IMagdaCallReferenceRepository
{
    private readonly IDocumentSession _session;

    public MagdaCallReferenceRepository(IDocumentSession session)
    {
        _session = session;
    }

    public async Task Save(MagdaCallReference magdaCallReference, CancellationToken cancellationToken)
    {
        _session.Store(magdaCallReference);
        await _session.SaveChangesAsync(cancellationToken);
    }
}
