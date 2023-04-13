namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;

[Serializable]
public class OutOfRangeVCode : InvalidVCode
{
    public OutOfRangeVCode() : base("VCode moet groter zijn dan 1000")
    {
    }

    protected OutOfRangeVCode(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
