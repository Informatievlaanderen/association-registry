namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class MultiplePrimaireLocaties : DomainException
{
    public MultiplePrimaireLocaties() : base("Er kan maar één primaire locatie zijn binnen de vereniging.")
    {
    }

    protected MultiplePrimaireLocaties(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
