namespace AssociationRegistry.DecentraalBeheer.Vereniging.Emails.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class EmailHeeftEenOngeldigFormaat : DomainException
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
