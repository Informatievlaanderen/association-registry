namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class StartdatumIsInFuture : DomainException
{
    public StartdatumIsInFuture() : base(ExceptionMessages.StartdatumIsInFuture)
    {
    }

    protected StartdatumIsInFuture(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
