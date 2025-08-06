namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
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
