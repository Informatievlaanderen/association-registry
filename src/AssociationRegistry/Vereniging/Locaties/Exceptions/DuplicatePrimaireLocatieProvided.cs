namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class DuplicatePrimaireLocatieProvided : DomainException
{
    public DuplicatePrimaireLocatieProvided() : base("Er kan maar één primaire locatie zijn binnen de vereniging.")
    {
    }

    protected DuplicatePrimaireLocatieProvided(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
