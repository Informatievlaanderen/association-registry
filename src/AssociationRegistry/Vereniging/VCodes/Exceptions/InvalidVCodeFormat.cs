namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;

[Serializable]
public class InvalidVCodeFormat : InvalidVCode
{
    public InvalidVCodeFormat() : base(ExceptionMessages.InvalidVCodeFormat)
    {
    }

    protected InvalidVCodeFormat(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
