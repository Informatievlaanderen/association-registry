namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class VerenigingKanNietVerwijderdWorden : DomainException
{
    public VerenigingKanNietVerwijderdWorden() : base(ExceptionMessages.VerenigingKanNietVerwijderdWorden)
    {
    }

    protected VerenigingKanNietVerwijderdWorden(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
