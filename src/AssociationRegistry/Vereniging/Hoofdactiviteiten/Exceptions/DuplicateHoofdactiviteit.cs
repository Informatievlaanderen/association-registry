namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class DuplicateHoofdactiviteit : DomainException
{
    public DuplicateHoofdactiviteit() : base(ExceptionMessages.DuplicateHoofdactiviteit)
    {
    }

    protected DuplicateHoofdactiviteit(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
