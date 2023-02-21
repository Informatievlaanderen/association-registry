namespace AssociationRegistry.ContactInfo.Urls.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InvalidUrlStart : DomainException
{
    public InvalidUrlStart() : base("Url moet beginnen met 'http://' of 'https://'")
    {
    }

    protected InvalidUrlStart(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
