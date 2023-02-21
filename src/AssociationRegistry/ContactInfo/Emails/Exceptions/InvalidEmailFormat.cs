namespace AssociationRegistry.ContactInfo.Emails.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InvalidEmailFormat: DomainException
{
    public InvalidEmailFormat() : base("Email voldeed niet aan het verwachte formaat (naam@domein.vlaanderen). " +
                                       "In naam worden de volgende characters toegestaan '!#$%&'*+/=?^_`{|}~-', " +
                                       "in domein enkel '.' en '-'.")
    {
    }

    protected InvalidEmailFormat(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
