namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.SequenceGuarding;

using AssociationRegistry.Admin.Schema;
using AssociationRegistry.EventStore;
using JasperFx.Events.Projections;
using Marten;

public abstract class SequenceGuarder<T> : ISequenceGuarder<T>
    where T : IMetadata
{
    private readonly IDocumentStore _documentStore;
    private readonly string _exceptionMessage;
    private readonly ShardName _shardName;

    protected SequenceGuarder(IDocumentStore documentStore, string exceptionMessage, ShardName shardName)
    {
        _documentStore = documentStore;
        _exceptionMessage = exceptionMessage;
        _shardName = shardName;
    }

    public async Task ThrowIfSequenceNotReached(long? expectedSequence)
    {
        if (!await HasReachedSequence(_documentStore, expectedSequence))
            throw new UnexpectedAggregateVersionException(_exceptionMessage);
    }

    private async Task<bool> HasReachedSequence(IDocumentStore documentStore, long? expectedSequence)
    {
        if (expectedSequence == null)
            return true;

        var sequenceReached = await documentStore.Advanced.ProjectionProgressFor(_shardName);

        return expectedSequence <= sequenceReached;
    }
}
