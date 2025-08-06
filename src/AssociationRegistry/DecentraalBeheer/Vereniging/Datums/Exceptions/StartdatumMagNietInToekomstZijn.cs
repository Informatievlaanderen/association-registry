namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class StartdatumMagNietInToekomstZijn : DomainException
{
    public StartdatumMagNietInToekomstZijn() : base(ExceptionMessages.StartdatumIsInFuture)
    {
    }

    protected StartdatumMagNietInToekomstZijn(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
