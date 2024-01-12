namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class HoofdactiviteitIsDuplicaat : DomainException
{
    public HoofdactiviteitIsDuplicaat() : base(ExceptionMessages.DuplicateHoofdactiviteit)
    {
    }

    protected HoofdactiviteitIsDuplicaat(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
