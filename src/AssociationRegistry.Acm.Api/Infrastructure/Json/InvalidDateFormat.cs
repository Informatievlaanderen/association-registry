namespace AssociationRegistry.Acm.Api.Infrastructure.Json;

using System;
using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InvalidDateFormat : DomainException
{
    public InvalidDateFormat() : base(ExceptionMessages.InvalidDateFormat)
    {
    }

    protected InvalidDateFormat(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
