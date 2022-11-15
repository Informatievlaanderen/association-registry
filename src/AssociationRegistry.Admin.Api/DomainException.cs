namespace AssociationRegistry.Admin.Api;

using System;
using System.Runtime.Serialization;

[Serializable]
public class DomainException : Exception
{
    public DomainException()
    {
    }

    public DomainException(string message) : base(message)
    {
    }

    public DomainException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
