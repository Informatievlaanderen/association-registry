namespace AssociationRegistry.VCodes.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public abstract class InvalidVCode : DomainException
{
    protected InvalidVCode(string message) : base(message)
    {
    }

    protected InvalidVCode(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
