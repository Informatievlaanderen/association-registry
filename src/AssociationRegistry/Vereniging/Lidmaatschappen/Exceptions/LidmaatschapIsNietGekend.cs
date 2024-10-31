namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

[Serializable]
public class LidmaatschapIsNietGekend : DomainException
{
    public LidmaatschapIsNietGekend(string lidmaatschapId) : base(string.Format(ExceptionMessages.LidmaatschapIsNietGekend, lidmaatschapId))
    {
    }

    protected LidmaatschapIsNietGekend(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
