namespace AssociationRegistry.Vereniging.Exceptions;

using System.Runtime.Serialization;
using Be.Vlaanderen.Basisregisters.AggregateSource;

[Serializable]
public class MaatschappelijkeZetelIsNotAllowed : DomainException
{
    public MaatschappelijkeZetelIsNotAllowed() : base(ExceptionMessages.MaatschappelijkeZetelIsNotAllowed)
    {
    }

    protected MaatschappelijkeZetelIsNotAllowed(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
