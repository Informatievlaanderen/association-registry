namespace AssociationRegistry.Vereniging.TelefoonNummers.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InvalidTelefoonNummerCharacter : DomainException
{
    public InvalidTelefoonNummerCharacter() : base(ExceptionMessages.InvalidTelefoonNummerCharacter)
    {
    }

    protected InvalidTelefoonNummerCharacter(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
