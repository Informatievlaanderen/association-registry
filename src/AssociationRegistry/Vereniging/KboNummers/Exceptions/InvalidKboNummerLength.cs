namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;

[Serializable]
public class InvalidKboNummerLength : InvalidKboNummer
{
    public InvalidKboNummerLength() : base("Kbo nummer moet 10 cijfers bevatten.")
    {
    }

    protected InvalidKboNummerLength(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
