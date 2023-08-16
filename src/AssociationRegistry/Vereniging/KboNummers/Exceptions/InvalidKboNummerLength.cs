namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;

[Serializable]
public class InvalidKboNummerLength : InvalidKboNummer
{
    public InvalidKboNummerLength() : base(ExceptionMessages.InvalidKboNummerLength)
    {
    }

    protected InvalidKboNummerLength(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
