namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

[Serializable]
public class IbanFormaatIsOngeldig: DomainException
{
    public IbanFormaatIsOngeldig() : base(ExceptionMessages.IbanFormaatIsOngeldig)
    {
    }

    protected IbanFormaatIsOngeldig(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
