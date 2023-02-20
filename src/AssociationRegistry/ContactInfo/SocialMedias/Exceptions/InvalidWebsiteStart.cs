namespace AssociationRegistry.ContactInfo.SocialMedias.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InvalidSocialMediaStart : DomainException
{
    public InvalidSocialMediaStart() : base("SocialMedia moet beginnen met 'http://' of 'https://'")
    {
    }

    protected InvalidSocialMediaStart(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
