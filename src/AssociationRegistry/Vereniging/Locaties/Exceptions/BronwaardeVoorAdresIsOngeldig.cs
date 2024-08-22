namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

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
