namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class StartdatumIsAfterEinddatum : DomainException
{
    public StartdatumIsAfterEinddatum() : base(ExceptionMessages.StartdatumIsAfterEinddatum)
    {
    }

    protected StartdatumIsAfterEinddatum(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
