namespace AssociationRegistry.Admin.Api.Magda;

using System.Threading;
using System.Threading.Tasks;
using AssociationRegistry.Magda.Models;
using Marten;

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
