namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;

[Serializable]
public class InvalidKboNummerChars : InvalidKboNummer
{
    public InvalidKboNummerChars() : base(ExceptionMessages.InvalidKboNummerChars)
    {
    }

    protected InvalidKboNummerChars(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
