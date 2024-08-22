namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class EinddatumMagNietInToekomstZijn : DomainException
{
    public EinddatumMagNietInToekomstZijn() : base(ExceptionMessages.EinddatumIsInFuture)
    {
    }

    protected EinddatumMagNietInToekomstZijn(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
