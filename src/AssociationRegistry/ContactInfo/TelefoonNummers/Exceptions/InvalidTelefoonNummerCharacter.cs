namespace AssociationRegistry.ContactInfo.TelefoonNummers.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InvalidTelefoonNummerCharacter : DomainException
{
    public InvalidTelefoonNummerCharacter() : base("TelefoonNummer moet bestaan uit cijfers, whitespace en \". /( ) - + \"")
    {
    }

    protected InvalidTelefoonNummerCharacter(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
