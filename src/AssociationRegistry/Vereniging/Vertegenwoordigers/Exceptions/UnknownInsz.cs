namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class UnknownInsz : DomainException
{
    public UnknownInsz() : base(ExceptionMessages.UnknownInsz)
    {
    }

    protected UnknownInsz(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
