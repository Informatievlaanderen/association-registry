namespace AssociationRegistry.Vereniging.Websites.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

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
