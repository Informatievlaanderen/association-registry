namespace AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;

using Resources;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

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
