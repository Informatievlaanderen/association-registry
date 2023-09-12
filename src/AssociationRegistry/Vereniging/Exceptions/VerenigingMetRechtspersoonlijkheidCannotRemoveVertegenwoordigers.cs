namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class VerenigingMetRechtspersoonlijkheidCannotRemoveVertegenwoordigers : DomainException
{
    public VerenigingMetRechtspersoonlijkheidCannotRemoveVertegenwoordigers() : base(ExceptionMessages.VerenigingMetRechtspersoonlijkheidCannotRemoveVertegenwoordigers)
    {
    }

    protected VerenigingMetRechtspersoonlijkheidCannotRemoveVertegenwoordigers(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}