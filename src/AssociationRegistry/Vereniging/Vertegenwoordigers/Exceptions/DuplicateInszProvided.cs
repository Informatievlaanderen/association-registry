namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class DuplicateInszProvided : DomainException
{
    public DuplicateInszProvided() : base(ExceptionMessages.DuplicateInszProvided)
    {
    }

    protected DuplicateInszProvided(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
