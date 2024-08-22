namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Resources;
using System.Runtime.Serialization;

[Serializable]
public class MaatschappelijkeZetelKanNietVerwijderdWorden : DomainException
{
    public MaatschappelijkeZetelKanNietVerwijderdWorden() : base(ExceptionMessages.MaatschappelijkeZetelCanNotBeRemoved)
    {
    }

    protected MaatschappelijkeZetelKanNietVerwijderdWorden(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
