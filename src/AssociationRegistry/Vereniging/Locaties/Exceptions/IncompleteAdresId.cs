namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class IncompleteAdresId : DomainException
{
    public IncompleteAdresId() : base(ExceptionMessages.IncompleteAdresId)
    {
    }

    protected IncompleteAdresId(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
