namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
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
