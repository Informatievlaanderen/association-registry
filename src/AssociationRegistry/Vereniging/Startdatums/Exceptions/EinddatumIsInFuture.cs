namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class EinddatumIsInFuture : DomainException
{
    public EinddatumIsInFuture() : base(ExceptionMessages.EinddatumIsInFuture)
    {
    }

    protected EinddatumIsInFuture(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
