namespace AssociationRegistry.Vereniging.Emails.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class EmailHeeftEenOngeldigFormaat: DomainException
{
    public EmailHeeftEenOngeldigFormaat() : base("E-mail voldoet niet aan het verwachte formaat (naam@domein.vlaanderen). " +
                                       "In naam worden de volgende tekens toegestaan '!#$%&'*+/=?^_`{|}~-', " +
                                       "in domein enkel '.' en '-'.")
    {
    }

    protected EmailHeeftEenOngeldigFormaat(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
