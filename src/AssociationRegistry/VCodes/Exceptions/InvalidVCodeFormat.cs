namespace AssociationRegistry.VCodes.Exceptions;

using System;
using System.Runtime.Serialization;

[Serializable]
public class InvalidVCodeFormat : InvalidVCode
{
    public InvalidVCodeFormat() : base("Formaat van de VCode moet 'V####' zijn")
    {
    }

    protected InvalidVCodeFormat(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
