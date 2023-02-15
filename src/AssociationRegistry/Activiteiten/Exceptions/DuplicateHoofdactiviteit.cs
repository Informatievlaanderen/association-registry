namespace AssociationRegistry.Activiteiten.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class DuplicateHoofdactiviteit: DomainException
{
    public DuplicateHoofdactiviteit() : base("Elke hoofdactiviteit moet uniek zijn")
    {
    }

    protected DuplicateHoofdactiviteit(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
