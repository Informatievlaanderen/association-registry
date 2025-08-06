namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

public class MeerderePrimaireVertegenwoordigers : DomainException
{
    public MeerderePrimaireVertegenwoordigers() : base(ExceptionMessages.MultiplePrimaireVertegenwoordigers)
    {
    }

    protected MeerderePrimaireVertegenwoordigers(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
