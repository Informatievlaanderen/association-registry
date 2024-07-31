namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class LocatieLengteIsOngeldig : DomainException
{
    public LocatieLengteIsOngeldig() : base(ExceptionMessages.UnsupportedOperationForLocatietype)
    {
    }

    protected LocatieLengteIsOngeldig(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
