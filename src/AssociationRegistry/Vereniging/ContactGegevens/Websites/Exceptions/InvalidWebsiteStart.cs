namespace AssociationRegistry.Vereniging.Websites.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InvalidWebsiteStart : DomainException
{
    public InvalidWebsiteStart() : base(ExceptionMessages.InvalidWebsiteStart)
    {
    }

    protected InvalidWebsiteStart(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
