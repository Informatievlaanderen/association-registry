namespace AssociationRegistry.INSZ.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InvalidInszLength:DomainException
{
    public InvalidInszLength() : base("INSZ moet 11 cijfers bevatten.")
    {
    }

    protected InvalidInszLength(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
