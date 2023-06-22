namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class DuplicateLocatieProvided : DomainException
{
    public DuplicateLocatieProvided() : base("Locaties moeten uniek zijn binnen de vereniging.")
    {
    }

    protected DuplicateLocatieProvided(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
