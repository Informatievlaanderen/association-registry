namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.SequenceGuarding;

using AssociationRegistry.Admin.Schema;

public interface ISequenceGuarder<T>
    where T : IMetadata
{
    Task ThrowIfSequenceNotReached(long? expectedSequence);
}
