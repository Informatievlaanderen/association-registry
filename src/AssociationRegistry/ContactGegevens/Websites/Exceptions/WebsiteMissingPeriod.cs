namespace AssociationRegistry.Contactgegevens.Websites.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class WebsiteMissingPeriod : DomainException
{
    public WebsiteMissingPeriod() : base("Website url moet minsens één punt bevatten")
    {
    }

    protected WebsiteMissingPeriod(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
