namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

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
