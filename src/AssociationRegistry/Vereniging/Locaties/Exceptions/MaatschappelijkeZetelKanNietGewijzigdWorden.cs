namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class MaatschappelijkeZetelKanNietGewijzigdWorden : DomainException
{
    public MaatschappelijkeZetelKanNietGewijzigdWorden() : base(ExceptionMessages.MaatschappelijkeZetelCanNotBeUpdated)
    {
    }

    protected MaatschappelijkeZetelKanNietGewijzigdWorden(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
