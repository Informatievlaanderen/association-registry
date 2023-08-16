namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class IncompleteAdres : DomainException
{
    public IncompleteAdres() : base(ExceptionMessages.IncompleteAdres)
    {
    }

    protected IncompleteAdres(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
