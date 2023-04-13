namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;

[Serializable]
public class InvalidKboNummerMod97 : InvalidKboNummer
{
    public InvalidKboNummerMod97() : base("Incorrect Kbo nummer: foutieve checksum.")
    {
    }

    protected InvalidKboNummerMod97(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
