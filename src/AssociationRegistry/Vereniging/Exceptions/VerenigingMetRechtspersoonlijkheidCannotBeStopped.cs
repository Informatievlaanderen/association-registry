namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class VerenigingMetRechtspersoonlijkheidCannotBeStopped : DomainException
{
    public VerenigingMetRechtspersoonlijkheidCannotBeStopped() : base(ExceptionMessages.VerenigingMetRechtspersoonlijkheidKanNietGestoptWorden)
    {
    }

    protected VerenigingMetRechtspersoonlijkheidCannotBeStopped(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
