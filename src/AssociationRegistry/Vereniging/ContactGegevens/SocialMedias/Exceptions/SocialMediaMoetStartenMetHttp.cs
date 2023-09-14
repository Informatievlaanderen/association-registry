namespace AssociationRegistry.Vereniging.SocialMedias.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class SocialMediaMoetStartenMetHttp : DomainException
{
    public SocialMediaMoetStartenMetHttp() : base(ExceptionMessages.InvalidSocialMediaStart)
    {
    }

    protected SocialMediaMoetStartenMetHttp(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
