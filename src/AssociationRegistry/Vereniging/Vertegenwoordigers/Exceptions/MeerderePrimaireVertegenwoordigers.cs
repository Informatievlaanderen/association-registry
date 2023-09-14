namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

public class MeerderePrimaireVertegenwoordigers : DomainException
{
    public MeerderePrimaireVertegenwoordigers() : base(ExceptionMessages.MultiplePrimaireVertegenwoordigers)
    {
    }

    protected MeerderePrimaireVertegenwoordigers(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
