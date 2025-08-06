namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class VerenigingMetRechtspersoonlijkheidKanNietGestoptWorden : DomainException
{
    public VerenigingMetRechtspersoonlijkheidKanNietGestoptWorden() : base(
        ExceptionMessages.VerenigingMetRechtspersoonlijkheidKanNietGestoptWorden)
    {
    }

    protected VerenigingMetRechtspersoonlijkheidKanNietGestoptWorden(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
