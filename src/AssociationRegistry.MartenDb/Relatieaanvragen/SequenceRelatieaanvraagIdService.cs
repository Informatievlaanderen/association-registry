namespace AssociationRegistry.MartenDb.Relatieaanvragen;

using DecentraalBeheer.Relatieaanvragen;
using Marten;
using VCodeGeneration;
using Weasel.Postgresql;

public class SequenceRelatieaanvraagIdService : IRelatieaanvraagIdService
{
    private readonly DocumentStore _documentStore;
    private readonly Sequence _sequence;

    public SequenceRelatieaanvraagIdService(IDocumentStore maybeDocumentStore)
    {
        if (maybeDocumentStore is not DocumentStore documentStore)
            throw new ArgumentException("IDocumentStore must be DocumentStore");

        _documentStore = documentStore;

        _sequence = _documentStore
            .StorageFeatures.FindFeature(typeof(RelatieaanvraagIdSequence))
            .Objects.OfType<Sequence>()
            .Single();
    }

    public async Task<RelatieaanvraagId> GetNext()
    {
        await using var session = _documentStore.LightweightSession();

        return RelatieaanvraagId.Create(await session.NextInSequence(_sequence));
    }
}
