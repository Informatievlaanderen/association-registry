namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InvalidDoelgroepRange : DomainException
{
    public InvalidDoelgroepRange() : base(ExceptionMessages.InvalidDoelgroepRange)
    {
    }

    protected InvalidDoelgroepRange(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
