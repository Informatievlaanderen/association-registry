namespace AssociationRegistry.Admin.Api.Infrastructure.Sequence;

public interface ISequenceGuarder
{
    Task ThrowIfSequenceNotReached(long? expectedSequence);
}
