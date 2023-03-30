﻿namespace AssociationRegistry.ContactGegevens.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InvalidContactType : DomainException
{
    public InvalidContactType() : base("Het opgegeven contacttype werd niet herkent. ('email', 'website', 'socialmedia', 'telefoon')")
    {
    }

    protected InvalidContactType(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
