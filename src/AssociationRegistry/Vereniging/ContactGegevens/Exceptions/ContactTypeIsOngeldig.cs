﻿namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class ContactTypeIsOngeldig : DomainException
{
    public ContactTypeIsOngeldig() : base(ExceptionMessages.InvalidContactType)
    {
    }

    protected ContactTypeIsOngeldig(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
