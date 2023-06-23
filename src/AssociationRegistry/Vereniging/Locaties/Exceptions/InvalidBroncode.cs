namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InvalidBroncode : DomainException
{
    public InvalidBroncode() : base("De broncode voor dit adres is niet gekend.")
    {
    }

    protected InvalidBroncode(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
