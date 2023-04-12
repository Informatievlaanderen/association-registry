﻿namespace AssociationRegistry.Contactgegevens.Emails.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InvalidEmailFormat: DomainException
{
    public InvalidEmailFormat() : base("Email voldoet niet aan het verwachte formaat (naam@domein.vlaanderen). " +
                                       "In naam worden de volgende tekens toegestaan '!#$%&'*+/=?^_`{|}~-', " +
                                       "in domein enkel '.' en '-'.")
    {
    }

    protected InvalidEmailFormat(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
