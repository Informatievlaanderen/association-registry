namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class DuplicateInszProvided : DomainException
{
    public DuplicateInszProvided() : base("INSZ moet uniek zijn binnen de vereniging.")
    {
    }

    protected DuplicateInszProvided(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
