namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

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
