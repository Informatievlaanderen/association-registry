namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

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
