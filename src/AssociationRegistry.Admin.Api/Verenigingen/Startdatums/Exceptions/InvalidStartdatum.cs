namespace AssociationRegistry.Admin.Api.Verenigingen.Startdatums.Exceptions;

using System;
using System.Runtime.Serialization;

[Serializable]
public class InvalidStartdatum : Exception
{
    public InvalidStartdatum() : base("Startdatum mag niet in de toekomst liggen.")
    {
    }

    protected InvalidStartdatum(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
