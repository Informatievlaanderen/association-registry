namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

public class MultiplePrimaireVertegenwoordigers : DomainException
{
    public MultiplePrimaireVertegenwoordigers() : base(ExceptionMessages.MultiplePrimaireVertegenwoordigers)
    {
    }

    protected MultiplePrimaireVertegenwoordigers(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
