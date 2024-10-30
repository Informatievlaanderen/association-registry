namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
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
