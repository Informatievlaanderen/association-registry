namespace AssociationRegistry.Admin.Api.Infrastructure.Json;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

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
