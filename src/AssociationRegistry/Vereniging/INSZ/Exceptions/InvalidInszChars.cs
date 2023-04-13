namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InvalidInszChars : DomainException
{
    public InvalidInszChars() : base("Foutieve tekens in INSZ.")
    {
    }

    protected InvalidInszChars(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
