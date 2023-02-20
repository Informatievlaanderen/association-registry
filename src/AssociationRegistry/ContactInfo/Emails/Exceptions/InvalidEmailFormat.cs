namespace AssociationRegistry.ContactInfo.Emails.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InvalidEmailFormat: DomainException
{
    public InvalidEmailFormat() : base("Email voldeed niet aan de opgelegde regex.")
    {
    }

    protected InvalidEmailFormat(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
