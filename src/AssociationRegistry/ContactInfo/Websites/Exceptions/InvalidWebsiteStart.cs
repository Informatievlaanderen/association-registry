namespace AssociationRegistry.ContactInfo.Websites.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InvalidWebsiteStart : DomainException
{
    public InvalidWebsiteStart() : base("Website moet beginnen met 'http://' of 'https://'")
    {
    }

    protected InvalidWebsiteStart(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
