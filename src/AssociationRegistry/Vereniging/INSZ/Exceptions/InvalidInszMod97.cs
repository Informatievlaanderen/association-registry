namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class InvalidInszMod97 : DomainException
{
    public InvalidInszMod97() : base(ExceptionMessages.InvalidInszMod97)
    {
    }

    protected InvalidInszMod97(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
