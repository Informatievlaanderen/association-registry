namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

[Serializable]
public class WerkingsgebiedIsDuplicaat : DomainException
{
    public WerkingsgebiedIsDuplicaat() : base(ExceptionMessages.DuplicateWerkingsgebied)
    {
    }

    protected WerkingsgebiedIsDuplicaat(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
