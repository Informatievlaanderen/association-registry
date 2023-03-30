namespace AssociationRegistry.Magda.Exceptions;

using System.Runtime.Serialization;

[Serializable]
public class MagdaException : Exception
{
    public MagdaException()
    {
    }

    public MagdaException(string? message) : base(message)
    {
    }

    protected MagdaException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
