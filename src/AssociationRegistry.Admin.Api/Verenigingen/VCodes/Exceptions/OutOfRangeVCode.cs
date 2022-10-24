namespace AssociationRegistry.Admin.Api.Verenigingen.VCodes.Exceptions;

using System;
using System.Runtime.Serialization;

[Serializable]
public class OutOfRangeVCode : InvalidVCode
{
    internal OutOfRangeVCode() : base("VCode must be between 1 and 999999 (inclusive)")
    {
    }

    protected OutOfRangeVCode(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
