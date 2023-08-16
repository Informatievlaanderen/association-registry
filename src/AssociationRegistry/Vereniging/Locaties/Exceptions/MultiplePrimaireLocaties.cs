namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class MultiplePrimaireLocaties : DomainException
{
    public MultiplePrimaireLocaties() : base(ExceptionMessages.MultiplePrimaireLocaties)
    {
    }

    protected MultiplePrimaireLocaties(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
