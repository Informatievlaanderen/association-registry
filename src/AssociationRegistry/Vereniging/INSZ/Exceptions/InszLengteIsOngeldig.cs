namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

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
