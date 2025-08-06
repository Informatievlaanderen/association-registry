namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

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
