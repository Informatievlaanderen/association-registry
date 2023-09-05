namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class StartdatumIsInFuture : DomainException
{
    public StartdatumIsInFuture() : base(ExceptionMessages.StardatumIsInFuture)
    {
    }

    protected StartdatumIsInFuture(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
