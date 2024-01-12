namespace AssociationRegistry.Vereniging.TelefoonNummers.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class TelefoonNummerBevatOngeldigeTekens : DomainException
{
    public TelefoonNummerBevatOngeldigeTekens() : base(ExceptionMessages.InvalidTelefoonNummerCharacter)
    {
    }

    protected TelefoonNummerBevatOngeldigeTekens(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
