namespace AssociationRegistry.Vereniging.SocialMedias.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class SocialMediaMissingPeriod : DomainException
{
    public SocialMediaMissingPeriod() : base(ExceptionMessages.SocialMediaMissingPeriod)
    {
    }

    protected SocialMediaMissingPeriod(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
