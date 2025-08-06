namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class VerenigingMetRechtspersoonlijkheidKanGeenVertegenwoordigersVerwijderen : DomainException
{
    public VerenigingMetRechtspersoonlijkheidKanGeenVertegenwoordigersVerwijderen() : base(
        ExceptionMessages.VerenigingMetRechtspersoonlijkheidCannotRemoveVertegenwoordigers)
    {
    }

    protected VerenigingMetRechtspersoonlijkheidKanGeenVertegenwoordigersVerwijderen(SerializationInfo info, StreamingContext context) :
        base(info, context)
    {
    }
}
