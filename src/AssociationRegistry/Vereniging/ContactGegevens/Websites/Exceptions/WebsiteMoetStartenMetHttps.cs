namespace AssociationRegistry.Vereniging.Websites.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class WebsiteMoetStartenMetHttps : DomainException
{
    public WebsiteMoetStartenMetHttps() : base(ExceptionMessages.InvalidWebsiteStart)
    {
    }

    protected WebsiteMoetStartenMetHttps(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
