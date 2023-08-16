namespace AssociationRegistry.Vereniging.Websites.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class WebsiteMissingPeriod : DomainException
{
    public WebsiteMissingPeriod() : base(ExceptionMessages.WebsiteMissingPeriod)
    {
    }

    protected WebsiteMissingPeriod(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
