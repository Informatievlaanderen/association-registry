namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

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
