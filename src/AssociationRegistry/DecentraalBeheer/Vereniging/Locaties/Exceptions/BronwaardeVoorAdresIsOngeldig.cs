namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
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
