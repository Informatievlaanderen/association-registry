namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class DuplicateLocatie : DomainException
{
    public DuplicateLocatie() : base("Locaties moeten uniek zijn binnen de vereniging.")
    {
    }

    protected DuplicateLocatie(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
