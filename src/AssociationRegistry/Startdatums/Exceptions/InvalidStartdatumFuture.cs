namespace AssociationRegistry.Startdatums.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InvalidStartdatumFuture : DomainException
{
    public InvalidStartdatumFuture() : base("Startdatum mag niet in de toekomst liggen.")
    {
    }

    protected InvalidStartdatumFuture(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
