namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class LocatieTypeIsNotAllowed : DomainException
{
    public LocatieTypeIsNotAllowed() : base(ExceptionMessages.LocatieTypeIsNotAllowed)
    {
    }

    protected LocatieTypeIsNotAllowed(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
