namespace AssociationRegistry.ContactInfo.Urls.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class UrlMissingPeriod : DomainException
{
    public UrlMissingPeriod() : base("Url moet minsens één punt bevatten")
    {
    }

    protected UrlMissingPeriod(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
