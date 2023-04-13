namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class StardatumIsInFuture : DomainException
{
    public StardatumIsInFuture() : base("Startdatum mag niet in de toekomst liggen.")
    {
    }

    protected StardatumIsInFuture(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
