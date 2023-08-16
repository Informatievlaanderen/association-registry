namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InvalidInszChars : DomainException
{
    public InvalidInszChars() : base(ExceptionMessages.InvalidInszChars)
    {
    }

    protected InvalidInszChars(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
