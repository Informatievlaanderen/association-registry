namespace AssociationRegistry.Admin.Api.Verenigingen.VCodes.Exceptions;

using System;
using System.Runtime.Serialization;

[Serializable]
public class InvalidVCodeLength : InvalidVCode
{
    internal InvalidVCodeLength() : base("VCode must be of length 7")
    {
    }

    protected InvalidVCodeLength(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
