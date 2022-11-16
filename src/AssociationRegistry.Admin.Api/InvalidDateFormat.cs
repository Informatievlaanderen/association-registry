namespace AssociationRegistry.Admin.Api;

using System;
using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InvalidDateFormat : DomainException
{
    public InvalidDateFormat() : base("Datum moet van het formaat 'yyyy-MM-dd' zijn.")
    {
    }

    protected InvalidDateFormat(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
