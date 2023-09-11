namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class UnsupportedOperationForLocatietype : DomainException
{
    public UnsupportedOperationForLocatietype() : base(ExceptionMessages.UnsupportedOperationForLocatietype)
    {
    }

    protected UnsupportedOperationForLocatietype(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
