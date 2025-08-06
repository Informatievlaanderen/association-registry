namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class VerenigingMetRechtspersoonlijkheidKanGeenVertegenwoordigersToevoegen : DomainException
{
    public VerenigingMetRechtspersoonlijkheidKanGeenVertegenwoordigersToevoegen() : base(
        ExceptionMessages.VerenigingMetRechtspersoonlijkheidCannotAddVertegenwoordigers)
    {
    }

    protected VerenigingMetRechtspersoonlijkheidKanGeenVertegenwoordigersToevoegen(SerializationInfo info, StreamingContext context) :
        base(info, context)
    {
    }
}
