namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class MaatschappelijkeZetelCanNotBeUpdated : DomainException
{
    public MaatschappelijkeZetelCanNotBeUpdated() : base(ExceptionMessages.MaatschappelijkeZetelCanNotBeUpdated)
    {
    }

    protected MaatschappelijkeZetelCanNotBeUpdated(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
