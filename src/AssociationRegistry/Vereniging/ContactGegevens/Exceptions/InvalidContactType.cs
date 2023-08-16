﻿namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InvalidContactType : DomainException
{
    public InvalidContactType() : base(ExceptionMessages.InvalidContactType)
    {
    }

    protected InvalidContactType(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
