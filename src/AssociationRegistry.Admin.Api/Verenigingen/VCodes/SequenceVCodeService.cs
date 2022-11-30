namespace AssociationRegistry.Admin.Api.Verenigingen.VCodes;

using System;
using System.Linq;
using System.Threading.Tasks;
using AssociationRegistry.VCodes;
using Marten;
using Weasel.Postgresql;

public class SequenceVCodeService : IVCodeService
{
    private readonly DocumentStore _documentStore;
    private readonly Sequence _vCodeSequence;

    public SequenceVCodeService(IDocumentStore maybeDocumentStore)
    {
        if (maybeDocumentStore is not DocumentStore documentStore)
            throw new ArgumentException("IDocumentStore must be DocumentStore");

        _documentStore = documentStore;
        _vCodeSequence = _documentStore.StorageFeatures.FindFeature(typeof(VCodeSequence)).Objects.OfType<Sequence>().Single();
    }

    public async Task<VCode> GetNext()
    {
        await using var session = _documentStore.OpenSession();

        return VCode.Create(session.NextInSequence(_vCodeSequence));
    }
}
