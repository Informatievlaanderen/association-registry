namespace AssociationRegistry.Admin.Api.Verenigingen.VCodes.Exceptions;

using System;
using System.Runtime.Serialization;

[Serializable]
public class InvalidVCodeFormat : InvalidVCode
{
    internal InvalidVCodeFormat() : base("Format of VCode must be 'V000000'")
    {
    }

    protected InvalidVCodeFormat(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
