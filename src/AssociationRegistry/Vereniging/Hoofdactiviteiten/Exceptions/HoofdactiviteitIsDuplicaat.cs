namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

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
