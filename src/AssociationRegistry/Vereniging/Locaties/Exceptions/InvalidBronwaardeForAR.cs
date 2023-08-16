namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InvalidBronwaardeForAR : DomainException
{
    public InvalidBronwaardeForAR() : base(ExceptionMessages.InvalidBronwaardeForAR)
    {
    }

    protected InvalidBronwaardeForAR(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
