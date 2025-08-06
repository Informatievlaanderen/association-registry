namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
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
