namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

public class LidmaatschapIsOverlappend : DomainException
{
    public LidmaatschapIsOverlappend() : base(ExceptionMessages.LidmaatschapIsOverlappend)
    {
    }

    protected LidmaatschapIsOverlappend(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
