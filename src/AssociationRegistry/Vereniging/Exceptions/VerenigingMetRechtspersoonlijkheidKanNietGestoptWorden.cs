namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
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
