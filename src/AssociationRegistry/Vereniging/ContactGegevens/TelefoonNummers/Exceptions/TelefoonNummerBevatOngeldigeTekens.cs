namespace AssociationRegistry.Vereniging.TelefoonNummers.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

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
