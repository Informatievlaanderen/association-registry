namespace AssociationRegistry.Vereniging.Websites.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class WebsiteMoetMinstensEenPuntBevatten : DomainException
{
    public WebsiteMoetMinstensEenPuntBevatten() : base(ExceptionMessages.WebsiteMissingPeriod)
    {
    }

    protected WebsiteMoetMinstensEenPuntBevatten(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
