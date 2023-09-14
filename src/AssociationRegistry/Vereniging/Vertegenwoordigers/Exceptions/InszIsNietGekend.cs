namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InszIsNietGekend : DomainException
{
    public InszIsNietGekend() : base(ExceptionMessages.UnknownInsz)
    {
    }

    protected InszIsNietGekend(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
