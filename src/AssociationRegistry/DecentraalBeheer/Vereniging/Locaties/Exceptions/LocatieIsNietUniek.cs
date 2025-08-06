namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
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
