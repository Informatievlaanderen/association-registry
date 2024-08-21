namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

[Serializable]
public class StartdatumLigtNaEinddatum : DomainException
{
    public StartdatumLigtNaEinddatum() : base(ExceptionMessages.StartdatumIsAfterEinddatum)
    {
    }

    protected StartdatumLigtNaEinddatum(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
