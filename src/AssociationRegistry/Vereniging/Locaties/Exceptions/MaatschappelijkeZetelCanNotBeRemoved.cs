namespace AssociationRegistry.Vereniging.Exceptions;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using System.Runtime.Serialization;

[Serializable]
public class MaatschappelijkeZetelCanNotBeRemoved : DomainException
{
    public MaatschappelijkeZetelCanNotBeRemoved() : base(ExceptionMessages.MaatschappelijkeZetelCanNotBeRemoved)
    {
    }

    protected MaatschappelijkeZetelCanNotBeRemoved(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
