namespace AssociationRegistry.VCodes.Exceptions;

using System;
using System.Runtime.Serialization;

[Serializable]
public abstract class InvalidVCode : Exception
{
    protected InvalidVCode(string message) : base(message)
    {
    }

    protected InvalidVCode(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
