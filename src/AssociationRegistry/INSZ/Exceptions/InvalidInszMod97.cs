namespace AssociationRegistry.INSZ.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InvalidInszMod97:DomainException
{
    public InvalidInszMod97() : base("Incorrect INSZ: foutieve checksum.")
    {
    }

    protected InvalidInszMod97(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
