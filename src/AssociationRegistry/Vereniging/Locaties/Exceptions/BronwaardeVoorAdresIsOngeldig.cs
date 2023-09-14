namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class BronwaardeVoorAdresIsOngeldig : DomainException
{
    public BronwaardeVoorAdresIsOngeldig() : base(ExceptionMessages.InvalidBronwaardeForAR)
    {
    }

    protected BronwaardeVoorAdresIsOngeldig(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
