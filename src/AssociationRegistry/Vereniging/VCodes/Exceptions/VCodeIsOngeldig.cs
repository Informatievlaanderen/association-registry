namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public abstract class VCodeIsOngeldig : DomainException
{
    protected VCodeIsOngeldig(string message) : base(message)
    {
    }

    protected VCodeIsOngeldig(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
