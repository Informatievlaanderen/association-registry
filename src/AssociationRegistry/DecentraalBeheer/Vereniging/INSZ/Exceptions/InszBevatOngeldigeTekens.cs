namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class InszBevatOngeldigeTekens : DomainException
{
    public InszBevatOngeldigeTekens() : base(ExceptionMessages.InvalidInszChars)
    {
    }

    protected InszBevatOngeldigeTekens(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
