namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InvalidInszLength : DomainException
{
    public InvalidInszLength() : base(ExceptionMessages.InvalidInszLength)
    {
    }

    protected InvalidInszLength(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
