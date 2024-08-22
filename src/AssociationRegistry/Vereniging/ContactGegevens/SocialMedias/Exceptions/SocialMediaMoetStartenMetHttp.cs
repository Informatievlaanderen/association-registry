namespace AssociationRegistry.Vereniging.SocialMedias.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

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
