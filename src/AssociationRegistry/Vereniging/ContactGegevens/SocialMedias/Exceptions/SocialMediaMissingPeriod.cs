namespace AssociationRegistry.Vereniging.SocialMedias.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class SocialMediaMissingPeriod : DomainException
{
    public SocialMediaMissingPeriod() : base("Social media url moet minsens één punt bevatten")
    {
    }

    protected SocialMediaMissingPeriod(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
