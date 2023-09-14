namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InszLengteIsOngeldig : DomainException
{
    public InszLengteIsOngeldig() : base(ExceptionMessages.InvalidInszLength)
    {
    }

    protected InszLengteIsOngeldig(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
