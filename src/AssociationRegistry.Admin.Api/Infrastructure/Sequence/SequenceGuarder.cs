namespace AssociationRegistry.Admin.Api.Infrastructure.Sequence;

using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.EventStore;
using Marten;

public class SequenceGuarder : ISequenceGuarder
{
    private readonly IDocumentStore _documentStore;

    public SequenceGuarder(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task ThrowIfSequenceNotReached(long? expectedSequence)
    {
        if (!await _documentStore.HasReachedSequence<BeheerVerenigingDetailDocument>(expectedSequence))
            throw new UnexpectedAggregateVersionException(ValidationMessages.Status412Detail);
    }
}
