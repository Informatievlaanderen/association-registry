namespace AssociationRegistry.KboNummers.Exceptions;

using System.Runtime.Serialization;

[Serializable]
public class InvalidKboNummerChars : InvalidKboNummer
{
    public InvalidKboNummerChars() : base("Foutieve tekens in Kbo nummer.")
    {
    }

    protected InvalidKboNummerChars(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
