namespace AssociationRegistry.Integrations.Magda.Shared.Exceptions;

using Resources;
using System.Runtime.Serialization;

[Serializable]
public class MagdaException : Exception
{
    public MagdaException(): base(ExceptionMessages.MagdaException)
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
