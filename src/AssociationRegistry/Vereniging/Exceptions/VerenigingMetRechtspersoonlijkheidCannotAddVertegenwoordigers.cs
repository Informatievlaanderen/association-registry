namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class VerenigingMetRechtspersoonlijkheidCannotAddVertegenwoordigers : DomainException
{
    public VerenigingMetRechtspersoonlijkheidCannotAddVertegenwoordigers() : base(ExceptionMessages.VerenigingMetRechtspersoonlijkheidCannotAddVertegenwoordigers)
    {
    }

    protected VerenigingMetRechtspersoonlijkheidCannotAddVertegenwoordigers(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}