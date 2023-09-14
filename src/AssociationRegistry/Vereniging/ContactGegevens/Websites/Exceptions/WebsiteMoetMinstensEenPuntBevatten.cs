namespace AssociationRegistry.Vereniging.Websites.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

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
