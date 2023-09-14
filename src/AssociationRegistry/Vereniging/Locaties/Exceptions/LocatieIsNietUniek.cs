namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class LocatieIsNietUniek : DomainException
{
    public LocatieIsNietUniek() : base(ExceptionMessages.DuplicateLocatie)
    {
    }

    protected LocatieIsNietUniek(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
