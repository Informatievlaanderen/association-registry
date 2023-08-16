namespace AssociationRegistry.Vereniging.SocialMedias.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InvalidSocialMediaStart : DomainException
{
    public InvalidSocialMediaStart() : base(ExceptionMessages.InvalidSocialMediaStart)
    {
    }

    protected InvalidSocialMediaStart(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
