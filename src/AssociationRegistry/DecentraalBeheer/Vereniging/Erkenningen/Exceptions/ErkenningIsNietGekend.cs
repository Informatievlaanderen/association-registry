namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;

[Serializable]
public class ErkenningIsNietGekend : DomainException
{
    public ErkenningIsNietGekend(string erkenningId) : base(string.Format(ExceptionMessages.ErkenningIsNietGekend, erkenningId))
    {
    }

    protected ErkenningIsNietGekend(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
