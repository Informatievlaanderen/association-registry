namespace AssociationRegistry.Startdatums.Exceptions;

using System;
using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InvalidStartdatum : DomainException
{
    public InvalidStartdatum() : base("Startdatum mag niet in de toekomst liggen.")
    {
    }

    protected InvalidStartdatum(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
