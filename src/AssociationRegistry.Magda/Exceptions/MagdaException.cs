namespace AssociationRegistry.Magda.Exceptions;

using Resources;
using System.Runtime.Serialization;

[Serializable]
public class MagdaException : Exception
{
    public MagdaException()
    {
    }

    public MagdaException(Exception? innerException) : base(ExceptionMessages.MagdaException, innerException)
    {
    }

    public MagdaException(string? message) : base(message)
    {
    }

    public MagdaException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected MagdaException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
