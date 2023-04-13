namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;

[Serializable]
public class InvalidVCodeFormat : InvalidVCode
{
    public InvalidVCodeFormat() : base("Formaat van de VCode moet 'V0000000' zijn")
    {
    }

    protected InvalidVCodeFormat(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
