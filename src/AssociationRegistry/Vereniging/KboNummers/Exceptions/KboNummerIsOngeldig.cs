namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public abstract class KboNummerIsOngeldig : DomainException
{
    protected KboNummerIsOngeldig(string message) : base(message)
    {
    }

    protected KboNummerIsOngeldig(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
