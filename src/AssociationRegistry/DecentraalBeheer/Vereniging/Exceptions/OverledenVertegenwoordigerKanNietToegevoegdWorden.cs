namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

[Serializable]
public class OverledenVertegenwoordigerKanNietToegevoegdWorden: DomainException
{
    public OverledenVertegenwoordigerKanNietToegevoegdWorden() : base(ExceptionMessages.OverledenVertegenwoordigerKanNietToegevoegdWorden)
    {
    }

    protected OverledenVertegenwoordigerKanNietToegevoegdWorden(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
