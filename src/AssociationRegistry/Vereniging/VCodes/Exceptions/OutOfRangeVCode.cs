namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;

[Serializable]
public class OutOfRangeVCode : InvalidVCode
{
    public OutOfRangeVCode() : base(ExceptionMessages.OutOfRangeVCode)
    {
    }

    protected OutOfRangeVCode(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
