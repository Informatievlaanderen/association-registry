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
[Serializable]
public class MaatschappelijkeZetelCanNotBeRemoved : DomainException
{
    public MaatschappelijkeZetelCanNotBeRemoved() : base(ExceptionMessages.MaatschappelijkeZetelCanNotBeUpdated)
    {
    }

    protected MaatschappelijkeZetelCanNotBeRemoved(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
